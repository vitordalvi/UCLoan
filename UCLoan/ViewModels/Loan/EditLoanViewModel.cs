using System.ComponentModel.DataAnnotations;
using UCLoan.Constants;

namespace UCLoan.ViewModels.Loan
{
    public class EditLoanViewModel
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public EquipmentConstants.EquipmentLoanStatus LoanStatus { get; set; }
        public EquipmentConstants.EquipmentPhysicalStatus PhysicalStatus { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
