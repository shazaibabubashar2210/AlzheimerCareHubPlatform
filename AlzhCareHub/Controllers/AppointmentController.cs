using Microsoft.AspNetCore.Mvc;
using AlzhCareHub.Models;
using Firebase.Database;
using Firebase.Database.Query;
using AlzhCareHub.Services;
namespace AlzhCareHub.Controllers
{

    public class AppointmentController : Controller
    {
        private static string DatabaseUrl = "https://alzheimercarehubsystem-a42e6-default-rtdb.firebaseio.com/";
        private static FirebaseClient firebaseClient = new FirebaseClient(DatabaseUrl);

        [HttpPost]
        public async Task<IActionResult> Book([FromBody] AppointmentModel appointment)
        {
            try
            {
                if (appointment == null || string.IsNullOrEmpty(appointment.CaregiverEmail))
                    return BadRequest(new { error = "Invalid appointment data. Caregiver email is required." });

                appointment.Status = "Pending";

                // Store in Firebase
                var newAppointment = await firebaseClient.Child("Appointments").PostAsync(appointment);
                appointment.Id = newAppointment.Key;

                await firebaseClient.Child("Appointments").Child(appointment.Id).PutAsync(appointment);

                // Fetch Doctor's Email
                var doctor = (await firebaseClient.Child("Doctors").OnceAsync<TeamMember>())
                    .FirstOrDefault(d => d.Object.Name == appointment.Doctor)?.Object;

                if (doctor != null)
                    await AppointmentEmailService.SendConfirmation(appointment, doctor.Email, appointment.CaregiverEmail);

                return Ok(new { message = "Appointment booked successfully!", appointmentId = appointment.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAppointment([FromBody] AppointmentModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
                return BadRequest(new { error = "Invalid appointment ID." });

            try
            {
                var appointmentRef = firebaseClient.Child("Appointments").Child(model.Id);
                var appointment = await appointmentRef.OnceSingleAsync<AppointmentModel>();

                if (appointment == null)
                    return NotFound("Appointment not found.");

                var doctor = (await firebaseClient.Child("Doctors").OnceAsync<TeamMember>())
                    .FirstOrDefault(d => d.Object.Name == appointment.Doctor)?.Object;

                if (doctor == null)
                    return NotFound("Doctor not found.");

                if (model.Status == "Confirmed")
                {
                    appointment.Status = "Confirmed";
                    await AppointmentEmailService.SendConfirmation(appointment, doctor.Email, appointment.CaregiverEmail);
                }
                else if (model.Status == "Doctor Suggested New Date" && model.SuggestedDate != null && !string.IsNullOrEmpty(model.SuggestedTime))
                {
                    appointment.Status = "Doctor Suggested New Date";
                    appointment.SuggestedDate = model.SuggestedDate;
                    appointment.SuggestedTime = model.SuggestedTime;
                    await AppointmentEmailService.SendRescheduleNotification(appointment, appointment.CaregiverEmail);
                }
                else
                {
                    return BadRequest(new { error = "Invalid status update." });
                }

                await appointmentRef.PutAsync(appointment);
                return Ok(new { message = "Appointment updated successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error confirming appointment: {ex.Message}" });
            }
        }


    }
}