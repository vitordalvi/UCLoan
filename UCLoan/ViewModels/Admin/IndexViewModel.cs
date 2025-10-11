using System.ComponentModel.DataAnnotations;
using UCLoan.Models;
using UCLoan.Services;

namespace UCLoan.ViewModels.Admin
{
    public class IndexViewModel
    {
        // Cards perfil
        [Display(Name = "Usuário")]
        string Name = string.Empty;

        // Cards empréstimos
        [Display(Name = "Total de Empréstimos")]
        public int TotalLoans { get; set; }
        [Display(Name = "Empréstimos Atuais")]
        public int CurrentLoans { get; set; }
        [Display(Name = "Empréstimos Atrasados")]
        public int LateLoans { get; set; }

        // Cards equipamentos
        [Display(Name = "Total de Equipamentos")]
        public int AllEquipments { get; set; }
        [Display(Name = "Equipamentos em manutenção")]
        public int MaintanceEquipments { get; set; }
    }
}
