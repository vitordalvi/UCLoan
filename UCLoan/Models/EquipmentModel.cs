using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UCLoan.Models
{
    [Table("EquipmentModels")]
    public class EquipmentModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }
}
