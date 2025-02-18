using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }
    }
}
