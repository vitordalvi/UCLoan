using System.ComponentModel.DataAnnotations;

namespace UCLoan.ViewModels.Admin
{
    public class IndexViewModel
    {
        // Cards usuários
        [Display(Name = "Usuários Cadastrados")]
        public int TotalUsers { get; set; } = 0;
        [Display(Name = "Administradores")]
        public int TotalAdmins { get; set; } = 0;
        [Display(Name = "Novos usuários")]
        public int NewUsers { get; set; } = 0;

        //// Cards empréstimos
        [Display(Name = "Total de Empréstimos")]
        public int TotalLoans { get; set; }
        [Display(Name = "Empréstimos Atuais")]
        public int CurrentLoans { get; set; }
        [Display(Name = "Empréstimos Atrasados")]
        public int LateLoans { get; set; }

        // Cards equipamentos
        [Display(Name = "Total de Equipamentos")]
        public int AllEquipments { get; set;  } 
        [Display(Name = "Equipamentos emprestados")]
        public int LoanedEquipments { get; set; }
        [Display(Name = "Equipamentos em manutenção")]
        public int MaintanceEquipments { get; set; }
        
    }
}
