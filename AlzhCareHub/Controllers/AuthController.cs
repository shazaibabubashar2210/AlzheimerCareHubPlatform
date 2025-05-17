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
        public async Task<ActionResult> Register(string email, string password)
        {
            try
            {
                string role= "Caregiver"; // Default role for new users
                var auth = await FirebaseAuthHelper.RegisterUser(email, password,role);

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

        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            try
            {
                // Check for hardcoded admin credentials
                if (email == "admin@gmail.com" && password == "admin123@")
                {
                    HttpContext.Session.SetString("UserEmail", email);
                    return RedirectToAction("AdminDashboard");
                }

                // Otherwise, authenticate using Firebase
                var auth = await FirebaseAuthHelper.LoginUser(email, password);

                if (auth != null)
                {
                    HttpContext.Session.SetString("UserEmail", email);
                    return RedirectToAction("CaregiverDashboard"); // Default for non-admin users
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Invalid Username or Password. Please try again.";
            }

            return View();
        }


        public ActionResult AdminDashboard()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserEmail = userEmail;
            return View();
        }


        [HttpGet]
        public IActionResult CaregiverDashBoard()
        {
            return View();
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


    }
}
