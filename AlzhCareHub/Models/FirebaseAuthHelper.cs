namespace AlzhCareHub.Models
{
    using Firebase.Auth;
    using Firebase.Database;
    using Firebase.Database.Query;
    using System.Threading.Tasks;
    using System;

    public class FirebaseAuthHelper
    {
        private static string ApiKey = "AIzaSyCWeoUTihsySNUteX_FWBu9mDMXJY57yUs";
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private static FirebaseAuthProvider authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseUrl);

        // Register User with Email Verification
        public static async Task<FirebaseAuthLink> RegisterUser(string email, string password, string role)
        {
            try
            {
                var authLink = await authProvider.CreateUserWithEmailAndPasswordAsync(email, password);

                // Send email verification
                await authProvider.SendEmailVerificationAsync(authLink.FirebaseToken);

                // Save user role in Firebase Database
                await firebaseClient
                    .Child("Users")
                    .Child(authLink.User.LocalId)
                    .PutAsync(new { Email = email, Role = role, EmailVerified = false });

                return authLink;
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.EmailExists)
                {
                    throw new InvalidOperationException("This email is already registered. Please log in or use another email.");
                }
                throw new Exception("Error creating user: " + ex.Message);
            }
        }
        // Login User with Email Verification Check
        public static async Task<FirebaseAuthLink> LoginUser(string email, string password)
        {
            try
            {
                var authLink = await authProvider.SignInWithEmailAndPasswordAsync(email, password);

                // Force refresh user data
                await Task.Delay(1000); // Small delay to allow Firebase to update
                var freshAuth = await authProvider.GetUserAsync(authLink.FirebaseToken);

                // Now check email verification status
                if (!freshAuth.IsEmailVerified)
                {
                    throw new InvalidOperationException("Your email is not verified. Please check your inbox and verify your email before logging in.");
                }

                return authLink;
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.InvalidEmailAddress || ex.Reason == AuthErrorReason.WrongPassword)
                {
                    throw new InvalidOperationException("Invalid username or password. Please try again.");
                }
                throw new Exception("Login failed: " + ex.Message);
            }
        }

        public static async Task<bool> IsAdmin(string userId)
        {
            var user = await firebaseClient
                .Child("Users")
                .Child(userId)
                .OnceSingleAsync<dynamic>();

            return user?.Role == "Admin";
        }
        // Get User Role from Firebase Database
        public static async Task<string> GetUserRole(string userId)
        {
            var user = await firebaseClient
                .Child("Users")
                .Child(userId)
                .OnceSingleAsync<dynamic>();

            return user?.Role ?? "Unknown"; // Default to "Unknown" if role not found
        }

        // Reset Password
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