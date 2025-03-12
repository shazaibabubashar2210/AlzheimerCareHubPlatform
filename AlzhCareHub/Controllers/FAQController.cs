using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class FAQController : Controller
    {
        public IActionResult FAQ()
        {
            return View();
        }
    }
}
