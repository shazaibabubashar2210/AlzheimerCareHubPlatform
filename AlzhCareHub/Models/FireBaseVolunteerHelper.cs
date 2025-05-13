using Firebase.Database;
using Firebase.Database.Query;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
namespace AlzhCareHub.Models
{
    public class FirebaseVolunteerHelper
    {
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseUrl);

        public static async Task<bool> SaveVolunteer(VolunteerModel volunteer)
        {
            try
            {
                await firebaseClient
                    .Child("Volunteers")
                    .PostAsync(volunteer);

                await MatchAndNotify(volunteer); // Match and notify after save

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving volunteer: {ex.Message}");
                return false;
            }
        }

        public static async Task MatchAndNotify(VolunteerModel volunteer)
        {
            var caregiverRequests = await firebaseClient
                .Child("CareGiverRequests")
                .OnceAsync<CareGiverRequest>();

            foreach (var request in caregiverRequests)
            {
                // Matching logic
                bool isDaysMatch = volunteer.AvailableDays >= request.Object.DaysNeeded;
                bool isEmergencyMatch = !request.Object.EmergencyRequired || volunteer.AvailableForEmergency;
                bool isPhysicalHelpNeeded = request.Object.TypeOfHelp?.ToLower().Contains("physical") ?? false;
                bool isExperienceHelpNeeded = request.Object.TypeOfHelp?.ToLower().Contains("experience") ?? false;
                bool isPhysicalMatch = !isPhysicalHelpNeeded || volunteer.CanAssistPhysically;
                bool isExperienceMatch = !isExperienceHelpNeeded || volunteer.HasExperience;

                if (isDaysMatch && isEmergencyMatch && isPhysicalMatch && isExperienceMatch)
                {
                    // Match found, save to VolunteerMatches
                    var match = new
                    {
                        Caregiver = request.Object,
                        Volunteer = volunteer,
                        MatchStatus = "Pending" // Initial status
                    };

                    var savedMatch = await firebaseClient
                        .Child("VolunteerMatches")
                        .PostAsync(match);

                    Console.WriteLine("Match found and saved.");

                    // Send email to caregiver
                    await SendEmailToCareGiver(request.Object, volunteer);
                }
            }
        }
        public static async Task<bool> UpdateMatchStatus(string caregiverName, string volunteerName, string status)
        {
            try
            {
                // Get all matches from Firebase
                var matches = await firebaseClient
                    .Child("VolunteerMatches")
                    .OnceAsync<dynamic>();

                // Find the match based on caregiver and volunteer names
                var match = matches.FirstOrDefault(m => m.Object.Caregiver?.Name == caregiverName && m.Object.Volunteer?.Name == volunteerName);

                if (match != null)
                {
                    // Update the match status
                    var updatedMatch = new
                    {
                        Caregiver = match.Object.Caregiver,
                        Volunteer = match.Object.Volunteer,
                        MatchStatus = status
                    };

                    // Update the match in Firebase
                    await firebaseClient
                        .Child("VolunteerMatches")
                        .Child(match.Key)  // Use the match key to update the right record
                        .PutAsync(updatedMatch);

                    // Notify admin and delete match from the dashboard if agreed
                    if (status == "Agreed")
                    { 
                        await firebaseClient.Child("VolunteerMatches").Child(match.Key).DeleteAsync();
                    }

                    return true; // Return true if the update was successful
                }

                return false; // Return false if no matching record was found
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating match status: {ex.Message}");
                return false; // Return false if there was an error
            }
        }



public static async Task NotifyAdminAboutAgreement(dynamic match)
    {
        var message = $"A match between caregiver {match.Caregiver.Name} and volunteer {match.Volunteer.Name} has been agreed.";

        // Notify the admin through SignalR

        // Optionally, send an email as well
        var adminEmail = "admin@example.com"; // Admin email
        var mail = new MailMessage
        {
            From = new MailAddress("shazaibabubashar@gmail.com"),
            Subject = "Volunteer-Caregiver Match Agreed!",
            Body = message
        };

        mail.To.Add(adminEmail);

        using (var smtpClient = new SmtpClient("smtp.gmail.com"))
        {
            smtpClient.Port = 587;
            smtpClient.Credentials = new System.Net.NetworkCredential("shazaibabubashar@gmail.com", "your-email-password");
            smtpClient.EnableSsl = true;

            await smtpClient.SendMailAsync(mail);
            Console.WriteLine("Admin notified about match agreement.");
        }
    }

    private static async Task SendEmailToCareGiver(CareGiverRequest caregiver, VolunteerModel volunteer)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("shazaibabubashar@gmail.com");
                mail.To.Add(caregiver.Email);
                mail.Subject = "🎉 Volunteer Match Found!";
                mail.Body = $"Dear {caregiver.Name},\n\n" +
                            $"A volunteer has matched your care request!\n\n" +
                            $"👤 Name: {volunteer.Name}\n📞 Phone: {volunteer.PhoneNumber}\n📧 Email: {volunteer.Email}\n" +
                            $"🕐 Available Days: {volunteer.AvailableDays}\n💪 Can Assist Physically: {volunteer.CanAssistPhysically}\n" +
                            $"🚨 Emergency Availability: {volunteer.AvailableForEmergency}\n" +
                            $"🧠 Experience: {volunteer.HasExperience}\n\n" +
                            $"Please reach out to the volunteer directly for assistance.\n\n" +
                            $"Warm regards,\nAlzhCareHub Team";

                smtpClient.Port = 587;
                smtpClient.Credentials = new System.Net.NetworkCredential("shazaibabubashar@gmail.com", "zkmo mgbv wnvt iemo");
                smtpClient.EnableSsl = true;

                await smtpClient.SendMailAsync(mail);

                Console.WriteLine("✅ Email sent to caregiver.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error sending email: {ex.Message}");
            }
        }
    }
}
