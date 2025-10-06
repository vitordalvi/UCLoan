using Microsoft.AspNetCore.Identity;
using UCLoan.Models;

namespace UCLoan.Repositories
{
    public interface IAdminRepository
    {
        Task<List<ApplicationUser>> GetUsersAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<IdentityResult> UpdateAsync(ApplicationUser user);

        Task<IList<string>> GetAllRolesAsync();
        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
        Task<IdentityResult> AddToRolesAsync(ApplicationUser user, IEnumerable<string> roles);
        Task<IdentityResult> RemoveFromRolesAsync(ApplicationUser user, IEnumerable<string> roles);
    }
}