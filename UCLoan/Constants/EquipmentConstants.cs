namespace UCLoan.Constants
{
    public class EquipmentConstants
    {
        public enum EquipmentLoanStatus
        {
            Available,
            Loaned,
            Reserved,
            Maintenance
        }

        public enum EquipmentPhysicalStatus
        {
            New,
            Good,
            Fair,
            Poor,
            Broken
        }
    }
}
