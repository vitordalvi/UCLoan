using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UCLoan.Constants;

namespace UCLoan.Models

{
    [Table("Equipment")]
    [Index(nameof(EquipmentId), IsUnique = true)]
    public class Equipment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O patrimônio é obrigatório.")]
        public int EquipmentId { get; set; }
        public EquipmentConstants.EquipmentLoanStatus LoanStatus { get; set; }
        [Required(ErrorMessage = "Selecione o estado físico.")]
        public EquipmentConstants.EquipmentPhysicalStatus PhysicalStatus { get; set; }
        [StringLength(255)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public int EquipmentModelId { get; set; }
        public EquipmentModel EquipmentModel { get; set; } = null!;
    }
}
