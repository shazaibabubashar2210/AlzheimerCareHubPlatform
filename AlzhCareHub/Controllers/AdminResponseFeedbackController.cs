using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class AdminResponseFeedbackController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
