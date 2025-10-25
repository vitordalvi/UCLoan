using UCLoan.Constants;

namespace UCLoan.Entities
{
    public class Loan
    {
        public Guid Id { get; set; }
        public Guid EquipmentId { get; set; }
        public LoanStatus Status { get; set; }
        public Equipment? Equipment { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
