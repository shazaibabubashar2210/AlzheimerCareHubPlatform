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
                var auth = await FirebaseAuthHelper.RegisterUser(email, password);

                if (auth != null)
                {
                    // Save the role after registration in the session
                    HttpContext.Session.SetString("UserRole", role);
                    HttpContext.Session.SetString("UserEmail", email);

                    TempData["Success"] = "Registration Successful! Please check your email for the verification link.";
                    return RedirectToAction("Login");
                }
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message; // Handle custom exception (invalid email, email exists)
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Registration Failed! " + ex.Message;
            }

            return View();
        }
        // Get a login page
        public ActionResult Login()
        {
            return View();
        }
        // Perform a login operation in this by getting the role 
        [HttpPost]
        public async Task<ActionResult> Login(string email, string password)
        {
            try
            {
                var auth = await FirebaseAuthHelper.LoginUser(email, password);

                if (auth != null)
                {
                    // Get the role stored in the session
                    var userRole = HttpContext.Session.GetString("UserRole");

                    // Redirect based on the user's role
                    if (userRole == "Caregiver")
                    {
                        return RedirectToAction("CaregiverDashboard"); // Caregiver-specific dashboard
                    }
                    else if (userRole == "Volunteer")
                    {
                        return RedirectToAction("VolunteerDashboard"); // Volunteer-specific dashboard
                    }
                    else
                    {
                        return RedirectToAction("Dashboard"); // Default dashboard or error view
                    }
                }
                else
                {
                    TempData["Error"] = "Login failed. Please try again.";
                    return View();
                }
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.WrongPassword)
                {
                    TempData["Error"] = "Invalid password. Please try again.";
                }
                else if (ex.Reason == AuthErrorReason.UnknownEmailAddress)
                {
                    TempData["Error"] = "Email not found. Please register first.";
                }
                else if (ex.Reason == AuthErrorReason.InvalidEmailAddress)
                {
                    TempData["Error"] = "Invalid email format. Please enter a valid email.";
                }
                else if (ex.Reason == AuthErrorReason.UserDisabled)
                {
                    TempData["Error"] = "This account has been disabled. Contact support.";
                }
                else
                {
                    TempData["Error"] = "Login failed. Please check your credentials.";
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An unexpected error occurred: " + ex.Message;
                return View();
            }
        }


        // Forgot password page
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // Handle password reset
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
        // Logout feature redirect to login page
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
            return View(); // Caregiver-specific dashboard view
        }

        public ActionResult VolunteerDashboard()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserEmail = userEmail;
            return View(); // Volunteer-specific dashboard view
        }
    }
}