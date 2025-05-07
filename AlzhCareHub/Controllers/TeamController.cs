
namespace AlzhCareHub.Controllers
{
    using AlzhCareHub.Models;
    using Google.Api;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    public class TeamController : Controller
    {
        private readonly TeamService _teamService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TeamController(IWebHostEnvironment webHostEnvironment)
        {
            _teamService = new TeamService();
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult AddDoctor()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddDoctor(TeamMember member)
        {
            if (member.ImageFile != null && member.ImageFile.Length > 0)
            {
                // Get root folder
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string folderPath = Path.Combine(wwwRootPath, "teamImages");

                // Create folder if it doesn’t exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Save the file
                string fileName = Path.GetFileName(member.ImageFile.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    

                    member.ImageFile.CopyTo(stream);
                }

                // Save the relative image path
                member.ImageUrl = Path.Combine("teamImages", fileName).Replace("\\", "/");
            }
            _teamService.AddTeamMember(member); 

            return RedirectToAction("DoctorAppointment", "DoctorAppointment"); // or return Ok if using API
        }

        public async Task<IActionResult> Index()
        {
            var teamMembers = await _teamService.GetTeamMembers(); // This already returns List<TeamMember>

            return View(teamMembers);
        }

    }
}
