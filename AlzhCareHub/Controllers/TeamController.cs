
namespace AlzhCareHub.Controllers
{
    using AlzhCareHub.Models;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class TeamController : Controller
    {
        private readonly TeamService _teamService;

        public TeamController()
        {
            _teamService = new TeamService();
        }

        [HttpGet]
        public IActionResult AddDoctor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(TeamMember member)
        {
            if (ModelState.IsValid)
            {
                bool isSuccess = await _teamService.AddTeamMember(member);
                if (isSuccess)
                {
                    return RedirectToAction("DoctorAppointment", "DoctorAppointment"); // Redirect to DoctorAppointment action in DoctorAppointmentController
                }
            }
            ViewBag.ErrorMessage = "Failed to add doctor.";
            return View(member);
        }
        public async Task<IActionResult> Index()
        {
            var teamMembers = await _teamService.GetTeamMembers(); // This already returns List<TeamMember>

            return View(teamMembers);
        }


    }


}
