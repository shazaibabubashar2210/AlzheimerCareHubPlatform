using AlzhCareHub.Models;
using Firebase.Database;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace AlzhCareHub.Controllers
{
    public class VolunteerController : Controller
    {
        private readonly FirebaseClient firebaseClient = new FirebaseClient("https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/");

        [HttpGet]
        public IActionResult Volunteer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Volunteer(VolunteerModel model)
        {
            if (ModelState.IsValid)
            {
                bool isSaved = await FirebaseVolunteerHelper.SaveVolunteer(model);
                if (isSaved)
                {
                    return RedirectToAction("VolunteerDashboard");
                }
                else
                {
                    TempData["Error"] = "There was an error saving your information. Please try again.";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> VolunteerDashboard()
        {
            var matches = await firebaseClient
                .Child("VolunteerMatches")
                .OnceAsync<dynamic>();

            var matchList = matches.Select(m => m.Object).ToList();

            return View(matchList);
        }
    }
}
