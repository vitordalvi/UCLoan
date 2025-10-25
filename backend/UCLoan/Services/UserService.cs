using System.Security.Claims;
using UCLoan.DTOs.User;
using UCLoan.Entities;
using UCLoan.Repository;

namespace UCLoan.Services
{
    public class UserService : BaseService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(IUserRepository userRepository, LogService logService, IHttpContextAccessor httpContextorAcessor) : base(logService)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextorAcessor;
        }

        // Método para pegar o ID do usuário logado a partir do token JWT
        private Guid? GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(idClaim, out var userId))
                return userId;

            return null;
        }

        // Mostrar as informações de perfil do usuário logado
        public async Task<(bool Success, string Message, GetMyProfileDTO? getProfileDto)> GetMyProfileAsync()
        {
            var userId = GetCurrentUserId();

            if (userId == null)
                return (false, "O usuário não foi encontrado pelo ID.", null);

            var user = await _userRepository.GetByIdAsync(userId.Value);

            if (user == null)
                return (false, "O usuário não existe.", null);


            var profileDto = new GetMyProfileDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CPF = user.CPF,
                CreatedAt = user.CreatedAt,
            };

            return (true, "Perfil encontrado com sucesso",  profileDto);
        }

        public async Task<(bool Success, string Message)> UpdateMyData(UpdateMyDataDTO dto)
        {
            // Lógica para atualizar o usuário
            var userId = GetCurrentUserId();

            if (userId == null)
                return (false, "O usuário não foi encontrado pelo ID.");

            var user = await _userRepository.GetByIdAsync(userId.Value);

            if (user == null)
                return (false, "O usuário não existe");

            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Name))
                return (false, "Email e nome são obrigatórios.");

            bool dtoEmailExists = await _userRepository.IsEmailInUse(dto.Email);
            bool changedEmail = dto.Email != user.Email;
            bool changedName = dto.Name != user.Name;

            if (!changedEmail && !changedName)
                return (true, "Não houveram mudanças nos seus dados.");

            if (changedEmail && dtoEmailExists)
                return (false, "O email já está em uso por outro usuário.");


            user.Email = dto.Email;
            user.Name = dto.Name;

            await _userRepository.UpdateAsync(user);
            var saved = await _userRepository.SaveChangesAsync();

            await _logService.LogAsync(
                user.Id,
                "O usuário atualizou os seus dados.",
                $"{user.Email}",
                user.Id,
                new { user.Email, user.Name });

            // Se email e nome foram alterados, ? "msg", se só email, : "msg", se só nome : "msg"
            string message = changedEmail && changedName 
                ? "Nome e email atualizados com sucesso." : changedEmail
                ? "Email atualizado com sucesso." : "Nome atualizado com sucesso.";

            return saved ? (true, message) : (false, "Erro ao atualizar os dados.");
        }
    }
}
