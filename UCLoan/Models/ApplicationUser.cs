using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace UCLoan.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "Nome completo: ")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "CPF:")]
        public string CPF { get; set; } = string.Empty;

        public ICollection<EquipmentHistory> ChangedHistories { get; set; } = new List<EquipmentHistory>();
        public ICollection<EquipmentHistory> LastUserHistories { get; set; } = new List<EquipmentHistory>();
    }
}
