namespace UCLoan.Entities
{
    public class Model
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public virtual ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
    }
}
