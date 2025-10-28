using UCLoan.Data;
using UCLoan.Utils;

namespace UCLoan.Services
{
    public abstract class BaseService
    {
        protected readonly LogService _logService;
        protected BaseService(LogService logService)
        {
            _logService = logService;
        }

        // Implementa a criação da log, a partir dos serviços que herdam essa classe base
        protected async Task LogActionAsync(
            Guid userId,
            string action,
            string entityName = "",
            Guid? entityId = null,
            object? details = null)
        {
            await _logService.LogAsync(userId, action, entityName, entityId, details);
        }
    }
}
