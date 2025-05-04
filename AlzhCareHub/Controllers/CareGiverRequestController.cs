using AlzhCareHub.Models;
using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class CareGiverRequestController : Controller
    {
        public IActionResult CareGiverRequest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CareGiverRequest(CareGiverRequest careGiverRequest)
        {
            bool result = await FirebaseCareGiverRequestHelper.SaveCareGiverRequest(careGiverRequest);

            if (result)
            {
                return RedirectToAction("CaregiverDashBoard", "Auth");

            }
            else
            {
                return View("Error"); // Display an error view or message
            }
        }
    }
}
