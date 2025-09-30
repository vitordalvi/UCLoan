using System.ComponentModel.DataAnnotations;

namespace UCLoan.Constants
{
    public class EquipmentConstants
    {
        public enum EquipmentLoanStatus
        {
            [Display(Name = "Indisponível")]
            Unavailable,

            [Display(Name = "Disponível")]
            Available,

            [Display(Name = "Emprestado")]
            Borrowed,

            [Display(Name = "Em atraso")]
            Overdue,

            [Display(Name = "Devolvido")]
            Returned,
        }

        public enum EquipmentPhysicalStatus
        {
            [Display(Name = "Excelente")]
            Excellent,

            [Display(Name = "Bom")]
            Good,

            [Display(Name = "Regular")]
            Fair,

            [Display(Name = "Ruim")]
            Poor,

            [Display(Name = "Quebrado")]
            Broken,
        }
    }
}
