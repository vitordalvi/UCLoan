namespace UCLoan.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public Guid? EntityId { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
        public string DetailsJson { get; set; } = string.Empty;
    }
}
