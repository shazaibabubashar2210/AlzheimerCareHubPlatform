
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

            return RedirectToAction("Index");
        }
        // Edit Doctor
        [HttpGet]
        public async Task<IActionResult> EditDoctor(string id)
        {
            var doctor = await _teamService.GetTeamMemberById(id);
            if (doctor == null)
                return NotFound();

            return View("AddDoctor", doctor); // Reuse the same view
        }

        [HttpPost]
        public async Task<IActionResult> EditDoctor(string id, TeamMember member)
        {
            if (member.ImageFile != null && member.ImageFile.Length > 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string folderPath = Path.Combine(wwwRootPath, "teamImages");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fileName = Path.GetFileName(member.ImageFile.FileName);
                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    member.ImageFile.CopyTo(stream);
                }

                member.ImageUrl = Path.Combine("teamImages", fileName).Replace("\\", "/");
            }

            await _teamService.UpdateTeamMember(id, member);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            await _teamService.DeleteTeamMember(id);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Index()
        {
            var teamMembers = await _teamService.GetTeamMembers(); // This already returns List<TeamMember>

            return View(teamMembers);
        }

    }
}
