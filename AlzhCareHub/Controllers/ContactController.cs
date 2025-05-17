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
            model.FirstName = model.FirstName?.Trim();
            model.LastName = model.LastName?.Trim();
            model.Email = model.Email?.Trim();
            model.PhoneNumber = model.PhoneNumber?.Trim();
            model.Message = model.Message?.Trim();

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
        [HttpGet]
        public async Task<IActionResult> AllContacts()
        {
            var contactList = await FireBaseContactHelper.GetAllContacts();
            return View(contactList); 
        }

    }
}
   

