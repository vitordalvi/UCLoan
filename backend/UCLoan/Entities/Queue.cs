using UCLoan.Constants;

namespace UCLoan.Entities
{
    public class Queue
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Position { get; set; }
        public QueuePriority Priority { get; set; } = QueuePriority.Normal;
        public QueueStatus Status { get; set; } = QueueStatus.Pending;
        public DateTime EnqueuedAt { get; set; } = DateTime.UtcNow;
    }
}
