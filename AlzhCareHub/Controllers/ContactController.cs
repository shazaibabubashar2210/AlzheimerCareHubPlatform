using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AlzhCareHub.Models;
using static System.Collections.Specialized.BitVector32;

namespace AlzhCareHub.Controllers
{
    public class ContactController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ContactModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    Console.WriteLine($"Field: {error.Key}, Errors: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                }
                return View("Index", model);
            }

            bool isSaved = await FireBaseContactHelper.SaveContact(model);
            if (isSaved)
            {
                TempData["Success"] = "Your message has been sent successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Failed to send a message!";
            }

            return View("Index", model);
        }

        // Use for a database test purposes to check whether it work or not...
        //public async Task<IActionResult> TestFirebase()
        //{
        //    ContactModel testModel = new ContactModel
        //    {
        //        FirstName = "Test",
        //        LastName = "User",
        //        Email = "test@example.com",
        //        PhoneNumber = "1234567890",
        //        Message = "This is a test message"
        //    };

        //    bool isSaved = await FireBaseContactHelper.SaveContact(testModel);
        //    return Content(isSaved ? "Firebase Save Successful" : "Firebase Save Failed");
        //}
    }
}
   

