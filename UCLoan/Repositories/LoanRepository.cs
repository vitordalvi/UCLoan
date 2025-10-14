using Microsoft.EntityFrameworkCore;
using UCLoan.Data;
using UCLoan.Models;

namespace UCLoan.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly ApplicationDbContext _context;

        public LoanRepository(ApplicationDbContext context)
        {
            _context = context;   
        }

        // Loans

        public  Task<List<Loan>> GetAllLoansAsync(CancellationToken ct = default) =>
            _context.Set<Loan>()
                .Include(l => l.User)
                .Include(l => l.Equipment)
                    .ThenInclude(e => e.EquipmentModel)
                .ToListAsync(ct);

        public Task<Loan?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _context.Set<Loan>()
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        public async Task AddAsync(Loan loan, CancellationToken ct = default)
        {
            await _context.Set<Loan>().AddAsync(loan, ct);
        }

        public Task UpdateAsync(Loan loan, CancellationToken ct = default)
        {
            _context.Set<Models.Loan>().Update(loan);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Loan loan, CancellationToken ct = default)
        {
            _context.Set<Loan>().Remove(loan);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
            _context.Set<Loan>().AnyAsync(e => e.Id == id, ct);

        public Task<bool> SaveChangesAsync(CancellationToken ct = default) =>
            _context.SaveChangesAsync(ct).ContinueWith(t => t.Result > 0, ct);
    }
}
