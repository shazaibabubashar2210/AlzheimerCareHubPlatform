using AlzhCareHub.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace AlzhCareHub.Controllers
{
    public class VolunteerController : Controller
    {
        public static readonly FirebaseClient firebaseClient = new FirebaseClient("https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/");


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string caregiverName, string volunteerName, string status)
        {
            bool isUpdated = await FirebaseVolunteerHelper.UpdateMatchStatus(caregiverName, volunteerName, status);
            return RedirectToAction("VolunteerDashboard");
        }

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



        public static async Task<bool> UpdateMatchStatus(string caregiverName, string volunteerName, string status)
        {
            try
            {
                var matches = await firebaseClient
                    .Child("VolunteerMatches")
                    .OnceAsync<dynamic>();

                var match = matches.FirstOrDefault(m => m.Object.Caregiver?.Name == caregiverName &&
                                                        m.Object.Volunteer?.Name == volunteerName);

                if (match != null)
                {
                    // If status is "Agreed", directly delete the match
                    if (status == "Agreed")
                    {
                        await firebaseClient.Child("VolunteerMatches").Child(match.Key).DeleteAsync();
                        Console.WriteLine("Match agreed and deleted.");
                    }
                    else
                    {
                        // Otherwise, update the status
                        var updatedMatch = new
                        {
                            Caregiver = match.Object.Caregiver,
                            Volunteer = match.Object.Volunteer,
                            MatchStatus = status
                        };

                        await firebaseClient
                            .Child("VolunteerMatches")
                            .Child(match.Key)
                            .PutAsync(updatedMatch);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating match status: {ex.Message}");
                return false;
            }
        }


    }
}
