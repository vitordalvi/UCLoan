namespace UCLoan.Repositories
{
    public interface IHomeRepository
    {
        Task<List<Models.Queue>> GetAllQueuesAsync(CancellationToken ct = default);
        Task<Models.Queue?> GetQueueByIdAsync(int id, CancellationToken ct = default);
        Task<int> GetTotalQueuesAsync(CancellationToken ct = default);
        Task<Models.Queue?> GetQueueByUserIdAsync(string userId, CancellationToken ct = default);
        Task<Models.Queue?> GetQueueByUserEmailAsync(string email, CancellationToken ct = default);
        Task<bool> RemoveQueueAsync(int id, CancellationToken ct = default);
        Task<bool> RemoveQueueByUserIdAsync(string userId, CancellationToken ct = default);
        Task<List<Models.Queue>> GetQueuesOrderedByCreationAsync(CancellationToken ct = default);
        Task AddQueueAsync(Models.Queue queue, CancellationToken ct = default);
        Task<int> GetQueuePositionByUserIdAsync(string userId, CancellationToken ct = default);
        Task<Models.Queue?> GetLatestQueueAsync(CancellationToken ct = default);
        Task<bool> IsUserInQueueAsync (string userId, CancellationToken ct = default);
        Task<int> GetUserPositionInQueueAsync(string userId, CancellationToken ct = default);
        Task<bool> SaveChangesAsync(CancellationToken ct = default);
    }
}
