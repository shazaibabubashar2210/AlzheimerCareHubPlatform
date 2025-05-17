using Microsoft.AspNetCore.Mvc;

namespace AlzhCareHub.Controllers
{
    public class ManageDoctorsController : Controller
    {
        public IActionResult ManageDoctor()
        {
            return View();
        }
    }
}
