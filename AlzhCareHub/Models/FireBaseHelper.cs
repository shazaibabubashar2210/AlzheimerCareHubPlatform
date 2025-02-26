using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlzhCareHub.Models
{
    public class FirebaseHelper
    {
        private static string ApiKey = "AIzaSyCWeoUTihsySNUteX_FWBu9mDMXJY57yUs";
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private static FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseUrl);

        // Submit Feedback
        public static async Task<bool> SubmitFeedback(FeedbackModel feedback)
        {
            try
            {
                feedback.Id = Guid.NewGuid().ToString(); // Assign unique ID
                await firebaseClient
                    .Child("Feedbacks")
                    .Child(feedback.Id)
                    .PutAsync(feedback);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Get All Feedbacks
        public static async Task<List<FeedbackModel>> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await firebaseClient
                    .Child("Feedbacks")
                    .OnceAsync<FeedbackModel>();

                List<FeedbackModel> feedbackList = new List<FeedbackModel>();
                foreach (var feedback in feedbacks)
                {
                    feedbackList.Add(feedback.Object);
                }

                return feedbackList;
            }
            catch (Exception)
            {
                return new List<FeedbackModel>();
            }
        }

        // Respond to Feedback
        public static async Task<bool> RespondToFeedback(string id, string responseMessage)
        {
            try
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
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
