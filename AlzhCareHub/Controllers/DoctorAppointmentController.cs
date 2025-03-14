using AlzhCareHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class DoctorAppointmentController : Controller
    {
        public async Task<IActionResult> DoctorAppointment()
        {
            var teamService = new TeamService();
            var members = await teamService.GetTeamMembers();
            return View(members);
        }
    }
}
