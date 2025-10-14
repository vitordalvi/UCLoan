namespace UCLoan.Repositories
{
    public interface ILoanRepository
    {
        Task<List<Models.Loan>> GetAllLoansAsync(CancellationToken ct = default);
        Task<Models.Loan?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Models.Loan loan, CancellationToken ct = default);
        Task UpdateAsync(Models.Loan loan, CancellationToken ct = default);
        Task DeleteAsync(Models.Loan loan, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
        Task<bool> SaveChangesAsync(CancellationToken ct = default);
    }
}
