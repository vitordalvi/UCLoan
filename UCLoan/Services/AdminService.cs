using UCLoan.Models;
using UCLoan.Repositories;

namespace UCLoan.Services
{
    public class AdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public Task<List<ApplicationUser>> GetUsersAsync() =>
            _adminRepository.GetUsersAsync();

        public Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName) =>
            _adminRepository.GetUsersInRoleAsync(roleName);

        public Task<ApplicationUser?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Task.FromResult<ApplicationUser?>(null);
            }
            return _adminRepository.GetByIdAsync(id);
        }

        public Task<List<ApplicationUser>> GetUserDateTime(DateTime? time = null) =>
            _adminRepository.GetUserDateTime(time);

        public Task<List<ApplicationUser>> GetNewUsers() =>
            _adminRepository.GetNewUsers(); 

        public async Task<(bool Success, IEnumerable<string> Erros)> UpdateDataAsync(ApplicationUser postedUser, IEnumerable<string?> selectedRoles)
        {
            var errors = new List<string>();

            var user = await _adminRepository.GetByIdAsync(postedUser.Id);
            if (user == null)
            {
                return (false, new[] { "Usuário não encontrado" });
            }

            user.FullName = postedUser.FullName;
            user.CPF = postedUser.CPF;
            user.PhoneNumber = postedUser.PhoneNumber;

            if (!string.Equals(user.Email, postedUser.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = postedUser.Email;
                user.UserName = postedUser.Email;
            }

            var updateResult = await _adminRepository.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                errors.AddRange(updateResult.Errors.Select(e => e.Description));
            }

            selectedRoles ??= Array.Empty<string>();
            var distinctSelected = selectedRoles.Where(r => !string.IsNullOrWhiteSpace(r))
                                               .Select(r => r!.Trim())
                                               .Distinct(StringComparer.OrdinalIgnoreCase)
                                               .ToList();

            foreach (var role in distinctSelected.ToList())
            {
                if (!await _adminRepository.RoleExistsAsync(role))
                {
                    errors.Add($"O seguinte cargo é inexistente: {role}");
                    distinctSelected.Remove(role);
                }
            }

            if (!errors.Any())
            {
                var currentRoles = await _adminRepository.GetUserRolesAsync(user);

                var addRole = distinctSelected.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();
                var removeRole = currentRoles.Except(distinctSelected, StringComparer.OrdinalIgnoreCase).ToList();

                if (addRole.Any())
                {
                    var addResult = await _adminRepository.AddToRolesAsync(user, addRole);
                    if (!addResult.Succeeded)
                        errors.AddRange(addResult.Errors.Select(e => e.Description));
                }

                if (removeRole.Any())
                {
                    var removeResult = await _adminRepository.RemoveFromRolesAsync(user, removeRole);
                    if (!removeResult.Succeeded)
                        errors.AddRange(removeResult.Errors.Select(e => e.Description));
                }
            }

            return (!errors.Any(), errors);
        }

        public Task<IList<string>> GetAllRolesAsync() =>
            _adminRepository.GetAllRolesAsync();

        public Task<IList<string>> GetUserRolesAsync(ApplicationUser user) =>
            _adminRepository.GetUserRolesAsync(user);

        public async Task<(bool Success, string? Error)> DeleteUserAsync(string id, string currentUserId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return (false, "Usuário não encontrado");
            }

            if (string.IsNullOrWhiteSpace(currentUserId))
            {
                return (false, "O Id do usuário autenticado é inválido.");
            }

            if (string.Equals(id, currentUserId, StringComparison.OrdinalIgnoreCase))
            {
                return (false, "Você não pode excluir o seu próprio usuário.");
            }
            
            var user = await _adminRepository.GetByIdAsync(id);

            if (user == null)
            {
                return (false, "O usuário não foi encontrado");
            }

            var result = await _adminRepository.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            // Se chegar aqui, a exclusão deu certo e não teve erros
            return (true, null);
        }
    }
}