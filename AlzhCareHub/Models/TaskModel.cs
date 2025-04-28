namespace AlzhCareHub.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TaskModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string Title { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } // Changed from PhoneNumber to Email

        [Required]
        public string Date { get; set; }

        [Required]
        public string Time { get; set; }

        [Required]
        public string Priority { get; set; }

        //[Required]
        public string Description { get; set; }
    }
}