using System.ComponentModel.DataAnnotations;

namespace AlzhCareHub.Models
{
    public class VolunteerModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Available days is required")]
        public int AvailableDays { get; set; }
        [Required(ErrorMessage = "Experience is required")]
        public bool HasExperience { get; set; }
        [Required]
        public bool CanAssistPhysically { get; set; }
        [Required]
        public bool AvailableForEmergency { get; set; }

    }
}
