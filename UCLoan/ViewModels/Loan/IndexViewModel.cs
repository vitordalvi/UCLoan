namespace UCLoan.ViewModels.Loan
{
    public class IndexViewModel
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = null!;
        public string EquipmentName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
