using UCLoan.Constants;

namespace UCLoan.Entities
{
    public class Equipment
    {
        public Guid Id { get; set; }
        public PhysicalStatus PhysicalStatus { get; set; }
        public LoanStatus LoanStatus { get; set; }
        public Guid ModelId { get; set; }
        public string Description { get; set; } = string.Empty;
        public Model? Model { get; set; }
    }
}
