using UCLoan.Entities;

namespace UCLoan.Repository
{
    public interface ILogRepository
    {
        // Interface para adicionar uma log no banco de dados
        Task AddLogAsync(ActivityLog log);
        // Interface para obter as logs de um usuário específico
        Task<List<ActivityLog>> GetUserLogsAsync(Guid userId);
    }
}
