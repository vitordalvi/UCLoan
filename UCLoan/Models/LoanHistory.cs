using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCLoan.Models
{
    [Table("LoanHistory")]
    public class LoanHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BorrowerName { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime? EndDate { get; set; }
        public string LoanNotes { get; set; } = string.Empty;
    }
}
