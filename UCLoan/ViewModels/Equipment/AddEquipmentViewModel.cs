using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using UCLoan.Constants;

namespace UCLoan.ViewModels.Equipment
{
    public class AddEquipmentViewModel
    {
        [Required(ErrorMessage = "O patrimônio é obrigatório.")]
        [Display(Name = "Patrimônio")]
        public int EquipmentId { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(255)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Selecione o estado físico.")]
        [Display(Name = "Estado Físico")]
        public EquipmentConstants.EquipmentPhysicalStatus PhysicalStatus { get; set; }

        [Required(ErrorMessage = "Selecione um modelo.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um modelo válido.")]
        [Display(Name = "Modelo do Equipamento")]
        public int EquipmentModelId { get; set; }
    }
}
