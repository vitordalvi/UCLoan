namespace UCLoan.ViewModels.Loan
{
    public class AddLoanViewModel
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public int EquipmentId { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsReturned { get; set; }

    }
}
