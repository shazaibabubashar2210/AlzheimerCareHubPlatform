using AlzhCareHub.Models;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AlzhCareHub.Services
{
    public static class AppointmentEmailService
    {
        public static async Task SendConfirmation(AppointmentModel appointment, string doctorEmail, string caregiverEmail)
        {
            string subject = "Appointment Confirmation";
            string body = $"Your appointment with Dr. {appointment.Doctor} on {appointment.AppointmentDate:yyyy-MM-dd} at {appointment.AppointmentTime} has been confirmed.";

            await SendEmail(doctorEmail, subject, body);
            await SendEmail(caregiverEmail, subject, body);
        }

        public static async Task SendRescheduleNotification(AppointmentModel appointment, string caregiverEmail)
        {
            string subject = "Appointment Rescheduled";
            string body = $"Dr. {appointment.Doctor} is unavailable at the requested time. They have suggested a new appointment on {appointment.SuggestedDate:yyyy-MM-dd} at {appointment.SuggestedTime}. Please confirm or reschedule.";

            await SendEmail(caregiverEmail, subject, body);
        }

        private static async Task SendEmail(string to, string subject, string body)
        {
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.Credentials = new System.Net.NetworkCredential("shazaibabubashar@gmail.com", "zkmo mgbv wnvt iemo");
                smtp.EnableSsl = true;

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("shazaibabubashar@gmail.com"),
                    Subject = subject,
                    Body = body
                };

                mail.To.Add(to);
                await smtp.SendMailAsync(mail);
            }
        }
    }
}
