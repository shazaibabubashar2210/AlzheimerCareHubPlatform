namespace AlzhCareHub.Models
{
    using Firebase.Auth;
    using System.Threading.Tasks;
    using System;

    public class FirebaseAuthHelper
    {
        private static string ApiKey = "AIzaSyD35i9-6Uw14gLu0WMoO_9Hm2qgX51qyR0";
        private static FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));

        public static async Task<FirebaseAuthLink> RegisterUser(string email, string password)
        {
            try
            {
                // Register the user
                var authLink = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

                // You will send the verification email from Firebase Admin or Cloud Functions here
                return authLink;
            }
            catch (FirebaseAuthException ex)
            {
                // Handle errors
                throw new Exception("Error creating user: " + ex.Message);
            }
        }

        public static async Task<FirebaseAuthLink> LoginUser(string email, string password)
        {
            return await authProvider.SignInWithEmailAndPasswordAsync(email, password);
        }

        public static async Task<string> GetUserRole(string email)
        {
            return "Caregiver"; // Placeholder, replace with your actual role logic
        }

        // Password reset logic
        public static async Task<string> ResetPassword(string email)
        {
            try
            {
                await authProvider.SendPasswordResetEmailAsync(email);
                return "A password reset email has been sent. Please check your inbox.";
            }
            catch (FirebaseAuthException ex)
            {
                throw new Exception("Error sending password reset email: " + ex.Message);
            }
        }
    }
}