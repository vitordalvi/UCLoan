using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UCLoan.Constants;

namespace UCLoan.Models

{
    [Table("Equipment")]
    public class Equipment
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public EquipmentConstants.EquipmentLoanStatus LoanStatus { get; set; }
        public EquipmentConstants.EquipmentPhysicalStatus PhysicalStatus { get; set; }
        public string Description { get; set; } = string.Empty;
        public EquipmentModel EquipmentModel { get; set; } = null!;
    }
}
