using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UCLoan.Models;
using UCLoan.Repositories;

namespace UCLoan.Services
{
    public class AdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminService(IAdminRepository adminRepository, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _adminRepository = adminRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            return await _userManager.FindByIdAsync(id);
        }

        public async Task<(bool Success, IEnumerable<string> Erros)> UpdateDataAsync(ApplicationUser postedUser, IEnumerable<string?> selectedRoles)
        {
            var errors = new List<string>();

            var user = await _userManager.FindByIdAsync(postedUser.Id);
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

            var updateResult = await _userManager.UpdateAsync(user);
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
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    errors.Add($"O seguinte cargo é inexistente: {role}");
                    distinctSelected.Remove(role);
                }
            }

            if (!errors.Any())
            {
                var currentRoles = await _userManager.GetRolesAsync(user);

                var addRole = distinctSelected.Except(currentRoles, StringComparer.OrdinalIgnoreCase).ToList();
                var removeRole = currentRoles.Except(distinctSelected, StringComparer.OrdinalIgnoreCase).ToList();

                if (addRole.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, addRole);
                    if (!addResult.Succeeded)
                        errors.AddRange(addResult.Errors.Select(e => e.Description));
                }

                if (removeRole.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, removeRole);
                    if (!removeResult.Succeeded)
                        errors.AddRange(removeResult.Errors.Select(e => e.Description));
                }
            }

            return (!errors.Any(), errors);
        }

        public async Task<IList<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}