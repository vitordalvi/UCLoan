using System.ComponentModel.DataAnnotations;

namespace UCLoan.Constants
{
    public class EquipmentConstants
    {
        public enum EquipmentLoanStatus
        {
            [Display(Name = "Indisponível")]
            Unavailable = 0,

            [Display(Name = "Disponível")]
            Available = 1,

            [Display(Name = "Emprestado")]
            Borrowed = 2,

            [Display(Name = "Em atraso")]
            Overdue = 3,

            [Display(Name = "Devolvido")]
            Returned = 4,
        }

        public enum EquipmentPhysicalStatus
        {
            [Display(Name = "Excelente")]
            Excellent = 0,

            [Display(Name = "Bom")]
            Good = 1,

            [Display(Name = "Regular")]
            Fair = 2,

            [Display(Name = "Ruim")]
            Poor = 3,

            [Display(Name = "Quebrado")]
            Broken = 4,
        }
    }
}
