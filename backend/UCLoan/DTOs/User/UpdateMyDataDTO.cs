using System.ComponentModel.DataAnnotations;
using UCLoan.Attributes;

namespace UCLoan.DTOs.User
{
    public class UpdateMyDataDTO
    {
        [Email]
        public string Email { get; set; } = string.Empty;
        [Name]
        public string Name { get; set; } = string.Empty;
    }
}
