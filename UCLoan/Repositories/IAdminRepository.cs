using Microsoft.AspNetCore.Identity;
using UCLoan.Models;

namespace UCLoan.Repositories
{
    public interface IAdminRepository
    {
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<List<ApplicationUser>> GetUsersInRoleAsync(string roleName);
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync(string email);
        Task<List<ApplicationUser>> GetUserDateTime(DateTime? time = null);
        Task<List<ApplicationUser>> GetNewUsers();
        Task<IdentityResult> UpdateAsync(ApplicationUser user);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<IList<string>> GetAllRolesAsync();
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<bool> RoleExistsAsync(string roleName);
    }
}