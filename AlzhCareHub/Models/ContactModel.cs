using System.ComponentModel.DataAnnotations;
namespace AlzhCareHub.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?\d{10,15}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Message cannot be empty.")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters long.")]
        public string Message { get; set; }
    }
}

