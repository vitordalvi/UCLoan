using UCLoan.DTOs.Admin;
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
        // Obtém a lista de todos os usuários
        public async Task<(bool Success, string Message, IList<GetUsersDto> dto)> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();

            if (users == null || !users.Any())
            {
                return (false, "Nenhum usuário encontrado", null!);
            }

            var usersDto = users.Select(user => new GetUsersDto
            {
                Guid = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            }).ToList();

            return (true, "Usuários cadastrados no sistema", usersDto);
        }
    }
}
