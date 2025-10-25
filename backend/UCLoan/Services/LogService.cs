using System.Text.Json;
using UCLoan.Entities;
using UCLoan.Repository;

namespace UCLoan.Services
{
    public class LogService
    {
        private readonly ILogRepository _logRepository;
        public LogService(ILogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        // Cria o registro da atividade (*Posso criar uma validação aqui depois, se for necessário realmente*)
        public async Task LogAsync(Guid userId, string action, string entityName = "", Guid? entityId = null, object? details = null)
        {
            var log = new ActivityLog
            {
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                DetailsJson = details != null ? JsonSerializer.Serialize(details) : string.Empty,
                OccurredAt = DateTime.UtcNow
            };

            await _logRepository.AddLogAsync(log);
        }
    }
}
