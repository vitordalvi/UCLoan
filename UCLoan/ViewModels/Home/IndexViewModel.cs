using System.ComponentModel.DataAnnotations;

namespace UCLoan.ViewModels.Home
{
    public class IndexViewModel
    {
        // Card dados do usuário
        [Display(Name = "Nome")]
        public string UserName { get; set; } = string.Empty;
        [Display(Name = "E-mail")]
        public string UserEmail { get; set; } = string.Empty;
        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; } = string.Empty;

        //// Card perfil do usuário
        [Display(Name = "Cargo do usuário")]
        public string Role { get; set; } = string.Empty;
        [Display(Name = "CPF do usuário")]
        public string CPF { get; set; } = string.Empty;

        // Cards empréstimo
        [Display(Name = "Empréstimo Ativo")]
        public int ActiveLoans { get; set; }
        [Display(Name = "ID do Empréstimo")]
        public int LoanId { get; set; }
        [Display(Name = "Data do Empréstimo")]
        public DateTime? LoanStartDate { get; set; }
        [Display(Name = "Data de Devolução")]
        public DateTime? LoanEndDate { get; set; }

        // Cards equipamentos
        [Display(Name = "Nome do Fabricante")]
        public string EquipmentManufacturer { get; set; } = string.Empty;
        [Display(Name = "Nome do Equipamento")]
        public string EquipmentName { get; set; } = string.Empty;
        [Display(Name = "ID do Equipamento")]
        public int EquipmentId { get; set; }
        [Display(Name = "Status do Equipamento")]
        public string EquipmentPhysicalStatus { get; set; } = string.Empty;

    }
}
