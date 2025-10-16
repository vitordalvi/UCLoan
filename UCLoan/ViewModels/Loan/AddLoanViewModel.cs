using UCLoan.Services;
using UCLoan.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UCLoan.ViewModels.Loan
{
    public class AddLoanViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O e-mail do recebedor é obrigatório.")]
        public string UserEmail { get; set; } = string.Empty;
        [Required(ErrorMessage = "O equipamento escolhido é obrigatório")]
        public int EquipmentId { get; set; }
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "A data de devolução é obrigatória.")]
        public DateTime? EndDate { get; set; }
        [StringLength(80, ErrorMessage = "A sua descrição deve conter um máximo de 80 caracteres.")]
        public string Description { get; set; } = string.Empty;

    }
}
