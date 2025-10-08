using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UCLoan.Constants;

namespace UCLoan.Models
{
    [Table("EquipmentHistory")]
    public class EquipmentHistory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; } = null!;
        [Required]
        public EquipmentConstants.EquipmentPhysicalStatus EquipmentPhysicalStatus { get; set; }
        [Required]
        public EquipmentConstants.EquipmentLoanStatus EquipmentLoanStatus { get; set; }
        public string? Notes { get; set; } = string.Empty;
        [Required]
        public DateTime ChangeDate { get; set; } = DateTime.UtcNow;
        [Required]
        public string ChangedById { get; set; }
        public ApplicationUser ChangedBy { get; set; } = null!;
        [Required]
        public string LastUserId { get; set; }
        public ApplicationUser LastUser { get; set; } = null!;

    }
}
