using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UCLoan.Models;

namespace UCLoan.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRepository(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<List<ApplicationUser>> GetUsersAsync() =>
            await _userManager.Users.ToListAsync();

        public async Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName) =>
            (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
        public Task<ApplicationUser?> GetByIdAsync(string id) =>
            _userManager.FindByIdAsync(id);

        public Task<ApplicationUser?> GetByEmailAsync(string email) =>
            _userManager.FindByEmailAsync(email);

        public async Task<List<ApplicationUser>> GetUserDateTime(DateTime? time = null)
        {
            // Se nenhum valor for fornecido, ele define o padrão como 7 dias atrás
            var defaultParam = time ?? DateTime.UtcNow.AddDays(-7);

            return await _userManager.Users
            .Where(e => e.CreatedAt >= defaultParam)
            .ToListAsync();
        }

        public async Task<List<ApplicationUser>> GetNewUsers()
        {
            return await _userManager.Users
                .Where(e => e.CreatedAt <= DateTime.UtcNow.AddDays(7))
                .ToListAsync();
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user) =>
            _userManager.UpdateAsync(user);
        public Task<IdentityResult> DeleteAsync(ApplicationUser user) =>
            _userManager.DeleteAsync(user);

        public async Task<IList<string>> GetAllRolesAsync() =>
            await _roleManager.Roles.Select(r => r.Name!).ToListAsync();

        public Task<IList<string>> GetUserRolesAsync(ApplicationUser user) =>
            _userManager.GetRolesAsync(user);

        public Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles) =>
            _userManager.AddToRolesAsync(user, roles);

        public Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles) =>
            _userManager.RemoveFromRolesAsync(user, roles);

        public Task<bool> RoleExistsAsync(string roleName) =>
            _roleManager.RoleExistsAsync(roleName);
    }
}