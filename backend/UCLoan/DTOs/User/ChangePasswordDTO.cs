using System.ComponentModel.DataAnnotations;
using UCLoan.Attributes;
using UCLoan.Constants.Utils;

namespace UCLoan.DTOs.User
{
    public class ChangePasswordDTO
    {
        [Required]
        public string OldPassword { get; set; } = string.Empty;
        [Required]
        [Password(PasswordPattern.DefaultPattern)]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [Password(PasswordPattern.DefaultPattern)]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
