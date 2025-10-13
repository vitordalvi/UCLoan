using System.ComponentModel.DataAnnotations;
using UCLoan.Constants;

namespace UCLoan.ViewModels.Equipment
{
    public class EditEquipmentViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Patrimônio")]
        public int EquipmentId { get; set; }
        [Display(Name = "Descrição")]
        [StringLength(255)]
        public string? Description { get; set; }
        [Display(Name = "Estado Físico")]
        public EquipmentConstants.EquipmentPhysicalStatus PhysicalStatus { get; set; }
        [Display(Name = "Estado Empréstimo")]
        public EquipmentConstants.EquipmentLoanStatus LoanStatus { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um modelo válido.")]
        [Display(Name = "Modelo do Equipamento")]
        public int EquipmentModelId { get; set; }
    }
}
