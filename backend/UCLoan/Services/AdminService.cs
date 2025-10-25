using UCLoan.Entities;
using UCLoan.Repository;

namespace UCLoan.Services
{
    public class AdminService : BaseService
    {
        private readonly IUserRepository _userRepository;
        public AdminService(IUserRepository userRepository, LogService logService) : base(logService)
        {
            _userRepository = userRepository;
        }

        // Obtém um usuário pelo seu ID
        public Task<User?> GetByIdAsync(Guid userId) =>
            _userRepository.GetByIdAsync(userId);
        // Obtem um usuário pelo seu email
        public Task<User?> GetByEmailAsync(string email) =>
            _userRepository.GetByEmailAsync(email);
    }
}
