using UCLoan.Constants;

namespace UCLoan.DTOs.Authentication
{
    public class UserRegisterDTO
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
