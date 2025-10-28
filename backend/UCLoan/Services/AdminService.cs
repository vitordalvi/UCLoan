using UCLoan.Constants;
using UCLoan.DTOs.Admin;
using UCLoan.Entities;
using UCLoan.Repository;
using UCLoan.Utils;

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
        public async Task<(bool Success, string Message, GetUserByIdDTO DTO)> GetByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                return (false, "O ID do usuário é inválido.", null!);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return (false, "O usuário não foi encontrado.", null!);

            var dto = new GetUserByIdDTO
            {
                UserId = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };

            return (true, $"Usuário encontrado com o ID ({user.Id.ToString()})", dto);
        }

        // Obtem um usuário pelo seu email
        public async Task<(bool Success, string Message, GetUserByEmailDTO DTO)> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "O email é inválido.", null!);

            var user = await _userRepository.GetByEmailAsync(email.Trim());

            if (user == null)
                return (false, "O usuário não foi encontrado.", null!);

            var dto = new GetUserByEmailDTO
            {
                UserId = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            };

            return (true, $"Usuário encontrado com o e-mail ({user.Email})", dto);
        }

        // Obtém a lista de todos os usuários
        public async Task<(bool Success, string Message, IList<GetUsersDTO> dto)> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();

            if (users == null || !users.Any())
            {
                return (false, "Nenhum usuário encontrado", null!);
            }

            var usersDto = users.Select(user => new GetUsersDTO
            {
                UserId = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt
            }).ToList();

            return (true, "Usuários cadastrados no sistema", usersDto);
        }

        public async Task<(bool Success, string Message, UpdateDataDTO? dto)> UpdateUserData(Guid userId, UpdateDataDTO dto)
        {
            var requirerId = await _userRepository.GetCurrentUserId();
            var requirer = await _userRepository.GetByIdAsync(requirerId);

            if (requirerId == Guid.Empty)
                return (false, "ID do requisitante inválido.", null);

            if (requirer == null)
                return (false, "Requisitante não encontrado.", null);

            if (userId == Guid.Empty)
                return (false, "ID do usuário inválido.", null);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return (false, "O usuário não foi encontrado.", null);

            if (dto == null)
                return (false, "Dados inválidos.", null);

            var name = dto.Name?.Trim();
            var email = dto.Email?.Trim();
            var cpf = dto.CPF?.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(cpf))
            {
                return (false, "Todos os campos são obrigatórios.", null!);
            }

            // Validação de Role
            if (!Enum.IsDefined(typeof(Role), dto.Role))
                return (false, "Função inválida.", null);

            // Verificar se e-mail está sendo usado
            var changedEmail = !string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase);
            var changedCpf = !string.Equals(user.CPF, cpf, StringComparison.OrdinalIgnoreCase);

            if (changedEmail && await _userRepository.IsEmailInUse(email))
                return (false, "O email já está em uso por outro usuário.", null);

            if (changedCpf && await _userRepository.IsCpfInUse(cpf))
                return (false, "O CPF já está em uso por outro usuário.", null);

            user.Email = email;
            user.Name = name;
            user.CPF = cpf;
            user.Role = dto.Role;

            var resultDto = new UpdateDataDTO
            {
                Name = user.Name,
                Email = user.Email,
                CPF = user.CPF,
                Role = user.Role
            };

            await _userRepository.Update(user);
            var saved = await _userRepository.SaveChangesAsync();

            await _logService.LogAsync(
                requirer.Id,
                "Admin atualizou os dados do usuário.",
                $"{user.Email}",
                user.Id,
                new { user.Email, user.Name, user.CPF, user.Role });

            return saved ? (true, "Dados alterados com sucesso.", resultDto) : (false, "Erro ao alterar os dados", null);
        }
    }
}
