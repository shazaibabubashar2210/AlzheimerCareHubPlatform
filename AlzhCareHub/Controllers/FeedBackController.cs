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
				feedback.Id = Guid.NewGuid().ToString();

				await firebaseClient
					.Child("Feedbacks")
					.Child(feedback.Id)
					.PutAsync(feedback);

				TempData["Message"] = "Thank You for your feedback!";
				return RedirectToAction("Responses", new { email = feedback.UserEmail });
			}

			ViewBag.Message = "Error submitting feedback!";
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
				return View(new List<FeedbackModel>());
			}

			return View(feedbackList.Select(f => f.Object).ToList());
		}

		// GET: View Only match email Responses for Users
		public async Task<ActionResult> Responses(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				TempData["ErrorMessage"] = "No email provided.";
				return RedirectToAction("Submit");
			}

			var feedbackList = await firebaseClient
				.Child("Feedbacks")
				.OnceAsync<FeedbackModel>();

			var filteredFeedback = feedbackList
				.Where(f => f.Object.UserEmail.Equals(email, StringComparison.OrdinalIgnoreCase))
				.Select(f => f.Object)
				.ToList();

			if (!filteredFeedback.Any())
			{
				TempData["ErrorMessage"] = "No feedback found for this email.";
			}

			return View(filteredFeedback);
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
					feedback.Response = null;

					await firebaseClient
						.Child("Feedbacks")
						.Child(id)
						.PutAsync(feedback);
				}
			}

			return RedirectToAction("Review");
		}

		// POST: Respond to feedback
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
					feedback.Response = responseMessage;

					await firebaseClient
						.Child("Feedbacks")
						.Child(id)
						.PutAsync(feedback);

					// ✅ FIX: Redirect with email to avoid ambiguous routing
					return RedirectToAction("Responses", new { email = feedback.UserEmail });
				}
			}

			return RedirectToAction("Review");
		}
	}
}
