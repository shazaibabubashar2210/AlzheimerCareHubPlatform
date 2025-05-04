using System.ComponentModel.DataAnnotations;

namespace AlzhCareHub.Models
{
    public class CareGiverRequest
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Patient's age is required")]
        public int PatientAge { get; set; }
        [Required(ErrorMessage = "Days needed is required")]
        public int DaysNeeded { get; set; }
        [Required(ErrorMessage = "Type of help is required")]
        public string TypeOfHelp { get; set; }
        [Required(ErrorMessage = "Emergency is required")]
        public bool EmergencyRequired { get; set; }
        public string AdditionalNotes { get; set; }
    }
}
