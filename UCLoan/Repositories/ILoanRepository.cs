using UCLoan.Constants;
using UCLoan.Models;

namespace UCLoan.Repositories
{
    public interface ILoanRepository
    {
        Task<List<Loan>> GetAllLoansAsync(CancellationToken ct = default);
        Task<EquipmentConstants.EquipmentLoanStatus> GetLoanStatusAsync(int id, CancellationToken ct = default);
        Task<Loan?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<Loan>> GetUserLoansByEmailAsync(string userEmail, CancellationToken ct = default);
        Task<List<Equipment>> GetUserLoansEquipmentByEmailAsync(string userEmail, CancellationToken ct = default);
        Task AddAsync(Loan loan, CancellationToken ct = default);
        Task UpdateAsync(Loan loan, CancellationToken ct = default);
        Task DeleteAsync(Loan loan, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<bool> SaveChangesAsync(CancellationToken ct = default);
    }
}
