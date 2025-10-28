using System.ComponentModel.DataAnnotations;
using UCLoan.Attributes;

namespace UCLoan.DTOs.Authentication
{
    public class UserLoginDTO
    {
        [Required]
        [Email]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Password(Constants.Utils.PasswordPattern.DefaultPattern)]
        public string Password { get; set; } = string.Empty;
    }
}
