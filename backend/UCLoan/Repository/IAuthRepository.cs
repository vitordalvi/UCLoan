using UCLoan.Entities;

namespace UCLoan.Repository
{
    public interface IAuthRepository
    {
        Task AddUserAsync(User user);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> SaveChangesAsync();
    }
}
