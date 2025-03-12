using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class DonationController : Controller
    {
        public IActionResult Donate()
        {
            return View();
        }
    }
}
