using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class ManageDonationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
