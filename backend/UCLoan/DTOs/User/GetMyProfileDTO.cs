using System.ComponentModel.DataAnnotations;
using UCLoan.Attributes;

namespace UCLoan.DTOs.User
{
    public class GetMyProfileDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string CPF { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
