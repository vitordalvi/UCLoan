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

        public Task<ApplicationUser?> GetByIdAsync(string id) =>
            _userManager.FindByIdAsync(id);

        public Task<IdentityResult> UpdateAsync(ApplicationUser user) =>
            _userManager.UpdateAsync(user);

        public async Task<IList<string>> GetAllRolesAsync() =>
            await _roleManager.Roles.Select(r => r.Name!).ToListAsync();

        public Task<IList<string>> GetUserRolesAsync(ApplicationUser user) =>
            _userManager.GetRolesAsync(user);

        public Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles) =>
            _userManager.AddToRolesAsync(user, roles);

        public Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles) =>
            _userManager.RemoveFromRolesAsync(user, roles);
    }
}