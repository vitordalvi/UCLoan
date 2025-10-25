using UCLoan.Entities;

namespace UCLoan.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid userId);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByCpfAsync(string CPF);
        Task UpdateAsync(User user);
        Task<bool> IsEmailInUse(string email);
        Task<bool> SaveChangesAsync();
    }
}
