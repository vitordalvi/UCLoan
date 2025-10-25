using System.ComponentModel.DataAnnotations;

namespace UCLoan.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid? LastLoanId { get; set; }
        public DateTime? LastActivityAt { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
    }
}
