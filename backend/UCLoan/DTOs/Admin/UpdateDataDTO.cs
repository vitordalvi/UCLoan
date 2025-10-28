using System.ComponentModel.DataAnnotations;
using UCLoan.Attributes;
using UCLoan.Constants;

namespace UCLoan.DTOs.Admin
{
    public class UpdateDataDTO
    {
        [Required]
        [Name]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Email]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Cpf]
        public string CPF { get; set; } = string.Empty;
        [Required]
        [Role]
        public Role Role { get; set; }
    }
}
