using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class DoctorAppointmentController : Controller
    {
        public IActionResult DoctorAppointment()
        {
            return View();
        }
    }
}
