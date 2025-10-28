using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using UCLoan.DTOs.User;
using UCLoan.Entities;
using UCLoan.Repository;

namespace UCLoan.Services
{
    public class UserService : BaseService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository, LogService logService) : base(logService)
        {
            _userRepository = userRepository;
        }

        // Mostrar as informações de perfil do usuário atual
        public async Task<(bool Success, string Message, GetMyProfileDTO?)> GetMyProfileAsync()
        {
            var userId = await _userRepository.GetCurrentUserId();

            if (userId == Guid.Empty)
                return (false, "O usuário não foi encontrado pelo ID.", null);

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return (false, "O usuário não foi encontrado.", null);

            var profileDto = new GetMyProfileDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CPF = user.CPF,
                Role = user.Role.ToString(),
                CreatedAt = user.CreatedAt,
            };

            return (true, "Perfil encontrado com sucesso",  profileDto);
        }

        // Alterar a senha do próprio usuário
        public async Task<(bool Success, string Message)> ChangePasswordAsync(
            string oldPassword,
            string newPassword,
            string confirmNewPassword)
        {
            var userId = await _userRepository.GetCurrentUserId();

            if (userId == Guid.Empty)
                return (false, "O usuário não foi encontrado pelo ID.");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                return (false, "O usuário não foi encontrado.");

            if (string.IsNullOrEmpty(oldPassword) ||
                string.IsNullOrEmpty(newPassword) ||
                string.IsNullOrEmpty(confirmNewPassword))
            {
                return (false, "Todos os campos são obrigatórios.");
            }

            var hasher = new PasswordHasher<User>();
            var verificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, oldPassword);

            if (oldPassword == newPassword)
                return (false, "A nova senha não pode ser igual à antiga.");

            if (verificationResult != PasswordVerificationResult.Success)
                return (false, "A senha antiga está incorreta.");

            if (newPassword != confirmNewPassword)
            {
                return (false, "A nova senha e a confirmação da nova senha não coincidem.");
            }

            user.PasswordHash = hasher.HashPassword(user, newPassword);
            await _userRepository.Update(user);

            var saved = await _userRepository.SaveChangesAsync();

            await _logService.LogAsync(
                user.Id,
                "O usuário alterou a sua senha.",
                $"{user.Email}",
                user.Id,
                new { user.Id, user.Email });

            return saved ? (true, "Senha alterada com sucesso.") : (false, "Erro ao alterar a senha.");
        }

        // Alterar os próprios dados (nome, email) do usuário
        public async Task<(bool Success, string Message)> UpdateMyData(UpdateMyDataDTO dto)
        {
            var userId = await _userRepository.GetCurrentUserId();
            if (userId == Guid.Empty)
                return (false, "O usuário não foi encontrado pelo ID.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "O usuário não existe");

            if (string.IsNullOrEmpty(dto.Email) && string.IsNullOrEmpty(dto.Name))
                return (false, "Email e nome são obrigatórios.");

            bool changedEmail = !string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase);
            bool changedName = !string.Equals(dto.Name, user.Name, StringComparison.Ordinal);

            var errors = new List<string>();
            var successes = new List<string>();

            // Validação de email
            if (changedEmail)
            {
                if (await _userRepository.IsEmailInUse(dto.Email))
                    errors.Add("O email já está em uso por outro usuário.");
                else
                {
                    user.Email = dto.Email;
                    successes.Add("Email atualizado com sucesso.");
                }
            }

            // Validação do nome
            if (changedName)
            {
                user.Name = dto.Name;
                successes.Add("Nome atualizado com sucesso.");
            }

            // Se não houve alterações
            if (!changedEmail && !changedName)
                return (true, "Não houveram mudanças nos seus dados.");

            // Salvar alterações que passaram na validação
            if (successes.Any())
            {
                await _userRepository.Update(user);
                var saved = await _userRepository.SaveChangesAsync();

                await _logService.LogAsync(
                    user.Id,
                    "O usuário atualizou os seus dados.",
                    $"{user.Email}",
                    user.Id,
                    new { user.Email, user.Name });

                if (!saved)
                    errors.Add("Erro ao atualizar os dados.");
            }

            // Mensagem final
            if (errors.Any() && successes.Any())
                return (false, string.Join(" ", errors) + " " + string.Join(" ", successes));
            if (errors.Any())
                return (false, string.Join(" ", errors));
            return (true, string.Join(" ", successes));
        }
    }
}
