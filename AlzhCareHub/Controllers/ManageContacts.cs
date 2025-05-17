using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class ManageContacts : Controller
    {
        public IActionResult ManageContact()
        {
            return View();
        }
    }
}
