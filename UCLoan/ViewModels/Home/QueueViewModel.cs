namespace UCLoan.ViewModels.Home
{
    public class QueueViewModel
    {
        public string UserEmail { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
    }
}
