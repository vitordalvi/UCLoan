using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using UCLoan.Models;

namespace UCLoan.ViewModels.Loan
{
    public class TakeOutRequestViewModel
    {
        public int QueueId { get; set; }

        [Required(ErrorMessage = "O e-mail do recebedor é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string UserEmail { get; set; } = string.Empty;

        [ValidateNever]
        public ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Selecione um equipamento.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um equipamento válido.")]
        public int EquipmentId { get; set; }

        [Required]
        public DateTime RequestedAt { get; set; }

        [Required(ErrorMessage = "A data de devolução é obrigatória.")]
        public DateTime EndDate { get; set; }

        [StringLength(80)]
        public string? Description { get; set; }
    }
}
