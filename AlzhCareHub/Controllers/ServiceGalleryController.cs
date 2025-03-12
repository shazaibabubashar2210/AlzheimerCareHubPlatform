using AlzhCareHub.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlzhCareHub.Controllers
{
    public class ServiceGalleryController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Remainder_And_Alerts()
        {
            var tasks = await FirebaseService.GetTasks();
            ViewBag.Tasks = tasks;
            return View(new TaskModel());
        }

        [HttpPost]
        public async Task<IActionResult> SaveTask(TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return View("Remainder_And_Alerts", task);
            }

            if (!IsValidEmail(task.Email))
            {
                ModelState.AddModelError("Email", "Invalid Email Address.");
                return View("Remainder_And_Alerts", task);
            }

            bool result = await FirebaseService.SaveTask(task);
            if (result)
            {
                return RedirectToAction("TaskList", new { successMessage = "Task added successfully!" });
            }

            ModelState.AddModelError("", "Failed to save the task. Please try again.");
            return View("Remainder_And_Alerts", task);
        }


        [HttpGet]
        public async Task<IActionResult> TaskList()
        {
            var tasks = await FirebaseService.GetTasks();
            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(string id)
        {
            await FirebaseService.DeleteTask(id);
            return RedirectToAction("TaskList");
        }
        [HttpGet]
        public async Task<IActionResult> EditTask(string id)
        {
            var task = await FirebaseService.GetTaskById(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // Edit Task
        [HttpPost]
        public async Task<IActionResult> EditTask(TaskModel task)
        {
            if (!ModelState.IsValid)
            {
                return View(task);
            }

            if (!IsValidEmail(task.Email))
            {
                ModelState.AddModelError("Email", "Invalid Email Address.");
                return View(task);
            }

            bool result = await FirebaseService.UpdateTask(task);
            if (result)
            {
                return RedirectToAction("TaskList", new { successMessage = "Task updated successfully!" });
            }

            ModelState.AddModelError("", "Failed to update the task. Please try again.");
            return View(task);
        }


        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
