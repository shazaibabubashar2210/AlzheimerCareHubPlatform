namespace AlzhCareHub.Models
{
    public class FeedbackModel
    {
        public string? Id { get; set; }
        public string? UserEmail { get; set; }
        public string? Message { get; set; }
        public string? Response { get; set; } // Admin Response
    }
}
