using System.ComponentModel.DataAnnotations;
using UCLoan.Attributes;

namespace UCLoan.DTOs.Admin
{
    public class GetUserByEmailDTO
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime CreatedAt { get; set; }
    }
}
