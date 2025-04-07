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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var firebaseClient = new FirebaseClient(DatabaseUrl);
            var appointments = await firebaseClient.Child("Appointments").OnceAsync<AppointmentModel>();

            List<AppointmentModel> appointmentList = appointments.Select(a => a.Object).ToList();

            return View(appointmentList);
        }



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

                // Redirect to Index page after successful booking
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmAppointment(string appointmentId)
        {
            if (string.IsNullOrEmpty(appointmentId))
            {
                Console.WriteLine("ERROR: appointmentId is missing.");
                return BadRequest("Appointment ID is required.");
            }

            Console.WriteLine($"Received appointmentId: {appointmentId}");

            var appointment = await firebaseClient.Child("Appointments").Child(appointmentId).OnceSingleAsync<AppointmentModel>();

            if (appointment == null)
                return NotFound("Appointment not found.");

            return View(appointment);
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

        // Cancelling An Appoint And removed from the database 
        [HttpPost]
        public async Task<IActionResult> CancelAppointment([FromBody] AppointmentModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
            {
                return BadRequest(new { error = "Invalid appointment ID" });
            }

            try
            {
                var firebaseClient = new FirebaseClient(DatabaseUrl);
                var appointmentToDelete = (await firebaseClient.Child("Appointments")
                    .OnceAsync<AppointmentModel>())
                    .FirstOrDefault(a => a.Object.Id == model.Id);

                if (appointmentToDelete != null)
                {
                    await firebaseClient.Child("Appointments")
                        .Child(appointmentToDelete.Key)
                        .DeleteAsync();
                }

                return Ok(new { message = "Appointment canceled successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> RescheduleAppointment([FromBody] AppointmentModel model)
        {
            if (string.IsNullOrEmpty(model.Id) || model.SuggestedDate == null || string.IsNullOrEmpty(model.SuggestedTime))
                return BadRequest(new { error = "Invalid reschedule request. Please provide a new date and time." });

            try
            {
                var appointmentRef = firebaseClient.Child("Appointments").Child(model.Id);
                var appointment = await appointmentRef.OnceSingleAsync<AppointmentModel>();

                if (appointment == null)
                    return NotFound("Appointment not found.");

                appointment.Status = "Caregiver Requested Reschedule";
                appointment.SuggestedDate = model.SuggestedDate;
                appointment.SuggestedTime = model.SuggestedTime;
                await appointmentRef.PutAsync(appointment);

                var doctor = (await firebaseClient.Child("Doctors").OnceAsync<TeamMember>())
                    .FirstOrDefault(d => d.Object.Name == appointment.Doctor)?.Object;


                return Ok(new { message = "Reschedule request sent successfully!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = $"Error rescheduling appointment: {ex.Message}" });
            }
        }


    }
}