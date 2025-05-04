using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

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

        private static async Task MatchAndNotify(VolunteerModel volunteer)
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
                    // Match found
                    await SendEmailToCareGiver(request.Object, volunteer);

                    // Save the match in Firebase
                    await firebaseClient
                        .Child("VolunteerMatches")
                        .PostAsync(new
                        {
                            Caregiver = request.Object,
                            Volunteer = volunteer
                        });

                    Console.WriteLine("Match found and email sent.");
                }
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
