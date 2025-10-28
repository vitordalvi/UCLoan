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
                return (false, "A nova senha não pode ser igual a antiga.");

            if (verificationResult != PasswordVerificationResult.Success)
                return (false, "A senha antiga está incoreta.");

            if (newPassword != confirmNewPassword)
            {
                return (false, "A nova senha e a confirmação da nova senha não coincidem.");
            }

            user.PasswordHash = hasher.HashPassword(user, newPassword);
            await _userRepository.UpdateAsync(user);

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
            // Lógica para atualizar o usuário
            var userId = await _userRepository.GetCurrentUserId();

            if (userId == Guid.Empty)
                return (false, "O usuário não foi encontrado pelo ID.");

            var user = await _userRepository.GetByIdAsync(userId);

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
