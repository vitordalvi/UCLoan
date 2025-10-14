using System.ComponentModel.DataAnnotations;

namespace UCLoan.ViewModels.Equipment
{
    public class EditModelViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Nome do Modelo")]
        [StringLength(60, ErrorMessage = "O nome do modelo não pode exceder 60 caracteres.")]
        public string Name { get; set; } = string.Empty;
        [Display(Name = "Fabricante")]
        [StringLength(50, ErrorMessage = "O nome do fabricante não pode exceder 50 caracteres.")]
        public string Manufacturer { get; set; } = string.Empty;
    }
}
