using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCLoan.Models
{
    [Table("Loans")]
    public class Loan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public ApplicationUser User { get; set; } = null!;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
    }
}
