using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class CareGiverRequestController : Controller
    {
        public IActionResult CareGiverRequest()
        {
            return View();
        }
    }
}
