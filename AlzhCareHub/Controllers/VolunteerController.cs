using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class VolunteerController : Controller
    {
        public IActionResult Volunteer()
        {
            return View();
        }
    }
}
