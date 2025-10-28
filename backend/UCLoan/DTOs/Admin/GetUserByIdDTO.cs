using System.ComponentModel.DataAnnotations;
using UCLoan.Attributes;

namespace UCLoan.DTOs.Admin
{
    public class GetUserByIdDTO
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [Name]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Email]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Role]
        public string Role { get; set; } = string.Empty;
        [Required]
        [DateRangeFromToday(-1, -1)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime CreatedAt { get; set; }

    }
}
