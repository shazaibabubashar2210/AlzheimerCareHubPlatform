namespace AlzhCareHub.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;
    using Firebase.Auth;
    using global::AlzhCareHub.Models;

    public class AuthController : Controller
    {
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(string email, string password, string role)
        {
            try
            {
                var auth = await FirebaseAuthHelper.RegisterUser(email, password, role);

                if (auth != null)
                {
                    TempData["Success"] = "Registration successful! Please check your email to verify your account.";
                    return RedirectToAction("Login");
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Registration Failed! " + ex.Message;
            }

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        // Login Action with Email Verification Check
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            try
            {
                var auth = await FirebaseAuthHelper.LoginUser(email, password);

                if (auth != null)
                {
                    var userRole = await FirebaseAuthHelper.GetUserRole(auth.User.LocalId);

                    HttpContext.Session.SetString("UserRole", userRole);
                    HttpContext.Session.SetString("UserEmail", email);

                    if (userRole == "Caregiver")
                    {
                        return RedirectToAction("CaregiverDashboard");
                    }
                    else if (userRole == "Volunteer")
                    {
                        return RedirectToAction("VolunteerDashboard");
                    }
                    else
                    {
                        return RedirectToAction("Dashboard");
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message; // Displaying user-friendly error message
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Invalid User Name And Password. Please try again.";
            }

            return View();
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            try
            {
                var result = await FirebaseAuthHelper.ResetPassword(email);
                TempData["Success"] = result;
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View();
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public ActionResult CaregiverDashboard()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserEmail = userEmail;
            return View();
        }

        public ActionResult VolunteerDashboard()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserEmail = userEmail;
            return View();
        }
    }
}