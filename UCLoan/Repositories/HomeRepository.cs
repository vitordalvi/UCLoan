using Microsoft.EntityFrameworkCore;
using System.Collections;
using UCLoan.Constants;
using UCLoan.Data;
using UCLoan.Models;
using UCLoan.Services;

namespace UCLoan.Repositories
{
    public class HomeRepository : IHomeRepository
    {
        private readonly ApplicationDbContext _context;
        public HomeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Models.Queue>> GetAllQueuesAsync(CancellationToken ct = default) =>
            _context.Set<Models.Queue>()
                .Include(q => q.User) // Exibir somente dados do usuário
                .AsNoTracking()
                .ToListAsync(ct);

        public Task<int> GetTotalQueuesAsync(CancellationToken ct = default) =>
            _context.Set<Models.Queue>()
                .CountAsync(ct);

        public Task<Models.Queue?> GetQueueByUserIdAsync(string userId, CancellationToken ct = default) =>
            _context.Set<Models.Queue>()
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.User!.Id == userId, ct);

        public async Task<Models.Queue?> GetQueueByUserEmailAsync(string email, CancellationToken ct = default) =>
            await _context.Set<Models.Queue>()
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.User!.Email == email, ct);
        public async Task<bool> RemoveQueueAsync(int id, CancellationToken ct = default)
        {
            var queue = await _context.Set<Models.Queue>()
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.Id == id, ct);

            if (queue != null)
            {
                _context.Set<Models.Queue>().Remove(queue);
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveQueueByUserIdAsync(string userId, CancellationToken ct = default)
        {
            var queue = await _context.Set<Models.Queue>()
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.User!.Id == userId, ct);
            if (queue != null)
            {
                _context.Set<Models.Queue>().Remove(queue);
                await _context.SaveChangesAsync(ct);
                return true;
            }
            return false;
        }

        public async Task<List<Models.Queue>> GetQueuesOrderedByCreationAsync(CancellationToken ct = default)
        {
            return await _context.Set<Models.Queue>()
                .Include(q => q.User)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<int> GetQueuePositionByUserIdAsync(string userId, CancellationToken ct = default)
        {
            var queues = await _context.Set<Models.Queue>()
                .Include(q => q.User)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync(ct);
            var position = queues.FindIndex(q => q.User!.Id == userId);
            return position >= 0 ? position + 1 : -1;
        }
        public async Task<Models.Queue?> GetLatestQueueAsync(CancellationToken ct = default) =>
            await _context.Set<Models.Queue>()
                .Include(q => q.User)
                .OrderByDescending(q => q.CreatedAt)
                .FirstOrDefaultAsync(ct);
        public async Task<Models.Queue?> GetQueueByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Set<Models.Queue>()
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.Id == id, ct);

        public Task<bool> IsUserInQueueAsync(string userId, CancellationToken ct = default) =>
            _context.Set<Models.Queue>()
                .AnyAsync(q => q.UserId == userId, ct);

        public async Task<int> GetUserPositionInQueueAsync(string userId, CancellationToken ct = default)
        {
            var queues = await _context.Set<Models.Queue>()
                .Include(q => q.User)
                .OrderBy(q => q.CreatedAt)
                .ToListAsync(ct);
            var position = queues.FindIndex(q => q.User!.Id == userId);
            return position >= 0 ? position + 1 : -1;
        }

        public async Task AddQueueAsync(Models.Queue queue, CancellationToken ct = default)
        {
            await _context.Set<Models.Queue>().AddAsync(queue, ct);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken ct = default) =>
            (await _context.SaveChangesAsync(ct)) > 0;
    }
}
