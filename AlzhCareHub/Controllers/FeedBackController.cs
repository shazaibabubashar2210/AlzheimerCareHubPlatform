using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlzhCareHub.Models;

namespace AlzhCareHub.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly FirebaseClient firebaseClient;

        public FeedbackController()
        {
            firebaseClient = new FirebaseClient("https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/");
        }

        // GET: Feedback Form
        public ActionResult Submit()
        {
            return View();
        }

        // POST: Submit Feedback
        [HttpPost]
        public async Task<ActionResult> Submit(FeedbackModel feedback)
        {
            if (ModelState.IsValid)
            {
                feedback.Id = Guid.NewGuid().ToString(); // Generate Unique ID

                await firebaseClient
                    .Child("Feedbacks")
                    .Child(feedback.Id)
                    .PutAsync(feedback);

                ViewBag.Message = "Thank You For your FeedBack!";
            }
            else
            {
                ViewBag.Message = "Error submitting feedback!";
            }

            return View();
        }

        // GET: Admin Review Feedback
        public async Task<ActionResult> Review()
        {
            var feedbackList = await firebaseClient
                .Child("Feedbacks")
                .OnceAsync<FeedbackModel>();

            if (feedbackList == null || !feedbackList.Any())
            {
                TempData["ErrorMessage"] = "No feedback found.";
                return View(new List<FeedbackModel>()); // Return an empty list instead of null
            }

            return View(feedbackList.Select(f => f.Object).ToList());
        }

        // GET: View All Responses for Users
        public async Task<ActionResult> Responses()
        {
            var feedbackList = await firebaseClient
                .Child("Feedbacks")
                .OnceAsync<FeedbackModel>();

            if (feedbackList == null || !feedbackList.Any())
            {
                TempData["ErrorMessage"] = "No feedback found.";
                return View(new List<FeedbackModel>()); // Return an empty list instead of null
            }

            return View(feedbackList.Select(f => f.Object).ToList());
        }

        // DELETE: Remove Response from Feedback
        [HttpPost]
        public async Task<ActionResult> DeleteResponse(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var feedback = await firebaseClient
                    .Child("Feedbacks")
                    .Child(id)
                    .OnceSingleAsync<FeedbackModel>();

                if (feedback != null)
                {
                    feedback.Response = null; // Remove response

                    await firebaseClient
                        .Child("Feedbacks")
                        .Child(id)
                        .PutAsync(feedback);
                }
            }

            return RedirectToAction("Review"); // Redirect back to Review page
        }
        [HttpPost]
        public async Task<ActionResult> Respond(string id, string responseMessage)
        {
            if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(responseMessage))
            {
                var feedback = await firebaseClient
                    .Child("Feedbacks")
                    .Child(id)
                    .OnceSingleAsync<FeedbackModel>();

                if (feedback != null)
                {
                    feedback.Response = responseMessage; // Store Admin Response

                    await firebaseClient
                        .Child("Feedbacks")
                        .Child(id)
                        .PutAsync(feedback);
                }
            }

            return RedirectToAction("Responses"); // Redirect back to Review page
        }

    }
}
