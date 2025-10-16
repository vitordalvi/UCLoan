using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UCLoan.Models
{
    [Table("Queue")]
    [Index(nameof(Id), IsUnique = true)]
    public class Queue
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Usuário que solicitou")]
        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Necessidade do Empréstimo")]
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Pedido em")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Display(Name = "Devolução")]
        public DateTime? ReturnDate { get; set; }
    }
}
