using System.Runtime.CompilerServices;
using UCLoan.Constants;
using UCLoan.Models;
using UCLoan.Repositories;

namespace UCLoan.Services
{
    public class HomeService
    {
        private readonly IHomeRepository _homeRepository;
        private readonly AdminService _adminService;
        private readonly EquipmentService _equipmentService;

        public HomeService(IHomeRepository homeRepository, AdminService adminService, EquipmentService equipmentService)
        {
            _homeRepository = homeRepository;
            _adminService = adminService;
            _equipmentService = equipmentService;
        }

        public Task<List<Models.Queue>> GetAllQueuesAsync(CancellationToken ct = default) =>
            _homeRepository.GetAllQueuesAsync(ct);

        public async Task<int> GetQueueCountAsync(CancellationToken ct = default)
        {
            var queues = await _homeRepository.GetAllQueuesAsync(ct);
            return queues.Count;
        }
        public Task<Models.Queue?> GetQueueByIdAsync(int id, CancellationToken ct = default) =>
            _homeRepository.GetQueueByIdAsync(id, ct);
        public Task<bool> IsUserInQueueAsync(string userId, CancellationToken ct = default) =>
            _homeRepository.IsUserInQueueAsync(userId, ct);
        public Task<bool> RemoveUserFromQueueAsync(string userId, CancellationToken ct = default) =>
            _homeRepository.RemoveQueueByUserIdAsync(userId, ct);

        public async Task<Models.Queue?> GetUserQueueAsync(string userId, CancellationToken ct = default)
        {
            var queues = await _homeRepository.GetAllQueuesAsync(ct);
            return queues.FirstOrDefault(q => q.User!.Id == userId);
        }

        public async Task<int> GetUserPositionInQueueAsync(string userId, CancellationToken ct = default)
        {
            var queues = await _homeRepository.GetAllQueuesAsync(ct);
            var orderedQueues = queues.OrderBy(q => q.CreatedAt).ToList();
            var position = orderedQueues.FindIndex(q => q.User!.Id == userId);

            return position >= 0 ? position + 1 : -1;
        }

        public async Task<(bool Success, string Error)> CreateQueueAsync(string userEmail,
            string description,
            DateTime createdAt,
            CancellationToken ct = default)
        {
            var user = await _adminService.GetByEmailAsync(userEmail);

            if (user == null)
            {
                return (false, "O seu usuário não foi encontrado.");
            }

            bool isInQueue = await IsUserInQueueAsync(user.Id, ct);

            if (isInQueue == true) 
            {
                return (false, "Você já está na fila.");
            }

                var queue = new Queue
                {
                    User = user,
                    UserId = user.Id,
                    Description = description,
                    CreatedAt = createdAt,
                };

            await _homeRepository.AddQueueAsync(queue, ct);
            var saved = await _homeRepository.SaveChangesAsync(ct);
            return saved ? (true, "Você entrou na fila com sucesso.") : (false, "Houve um erro ao entrar na fila.");
        }

        public async Task<(bool Success, string Error)> DeleteQueueAsync(int id, CancellationToken ct = default)
        {
            var queue = await _homeRepository.GetQueueByIdAsync(id);

            if (queue == null)
            {
                return (false, "A requisição da fila não foi encontrada.");
            }

            var user = queue.User;

            if (user == null) 
            {
                return (false, "A requisição não pertence a algum usuário.");
            }

            await _homeRepository.RemoveQueueAsync(queue.Id, ct);;
            var saved = await _homeRepository.SaveChangesAsync(ct);
            return saved ? (true, "A requisição foi recusada.") : (false, "Houve um erro ao recusar a requisição.");
        }
    }
}
