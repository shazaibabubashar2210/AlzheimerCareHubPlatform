using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Threading.Tasks;

namespace AlzhCareHub.Models
{
    public class FirebaseCareGiverRequestHelper
    {
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseUrl);

        // Method to save caregiver request data to Firebase
        public static async Task<bool> SaveCareGiverRequest(CareGiverRequest careGiverRequest)
        {
            try
            {
                await firebaseClient
                    .Child("CareGiverRequests")
                    .PostAsync(careGiverRequest);

                Console.WriteLine("Caregiver request saved successfully");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving caregiver request: {ex.Message}");
                return false;
            }
        }
    }
}
