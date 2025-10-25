using Microsoft.EntityFrameworkCore;
using UCLoan.Data;
using UCLoan.Entities;

namespace UCLoan.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly AppDbContext _context;
        public LogRepository(AppDbContext context)
        {
            _context = context;
        }

        // Adiciona uma log no banco de dados
        public async Task AddLogAsync(ActivityLog log)
        {
            await _context.ActivityLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        // Obtém as logs de um usuário específico
        public async Task<List<ActivityLog>> GetUserLogsAsync(Guid userId)
        {
            return await _context.ActivityLogs
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.OccurredAt)
                .ToListAsync();
        }
    }
}
