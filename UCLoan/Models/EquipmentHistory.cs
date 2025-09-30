using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UCLoan.Constants;

namespace UCLoan.Models
{
    [Table("EquipmentHistory")]
    public class EquipmentHistory
    {
        public int Id { get; set; }
        public Equipment Equipment { get; set; } = null!;
        public EquipmentConstants.EquipmentPhysicalStatus EquipmentPhysicalStatus { get; set; }
        public EquipmentConstants.EquipmentLoanStatus EquipmentLoanStatus { get; set; }
        public string? Notes { get; set; } = string.Empty;
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
        [Required]
        public string ChangedById { get; set; }
        public ApplicationUser ChangedBy { get; set; } = null!;
        [Required]
        public string LastUserId { get; set; }
        public ApplicationUser LastUser { get; set; } = null!;

    }
}
