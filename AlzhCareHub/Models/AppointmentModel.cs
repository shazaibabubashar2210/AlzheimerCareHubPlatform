namespace AlzhCareHub.Models
{
    public class AppointmentModel
    {
        public string Id { get; set; }
        public string Doctor { get; set; } // Doctor Name
        public string CaregiverEmail { get; set; } // Email of the person booking
        public DateTime AppointmentDate { get; set; } // Date of appointment
        public string AppointmentTime { get; set; } // Time of appointment
        public string Status { get; set; } // Pending, Confirmed, Rescheduled
        public DateTime? SuggestedDate { get; set; } // If doctor suggests new date
        public string SuggestedTime { get; set; } // If doctor suggests new time
    }
}
