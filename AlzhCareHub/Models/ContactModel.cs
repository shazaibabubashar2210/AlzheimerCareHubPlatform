using System.ComponentModel.DataAnnotations;

namespace AlzhCareHub.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage = "First name is required.")]
        [RegularExpression(@"^[A-Za-z]{2,50}$", ErrorMessage = "First name must contain only letters and be 2-50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [RegularExpression(@"^[A-Za-z]{2,50}$", ErrorMessage = "Last name must contain only letters and be 2-50 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?[0-9]{10,15}$", ErrorMessage = "Phone number must be 11 digits.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Message cannot be empty.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 1000 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s.,!?'-]{10,1000}$", ErrorMessage = "Message can only contain letters, numbers, and basic punctuation.")]
        public string Message { get; set; }
    }
}
