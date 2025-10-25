using Microsoft.EntityFrameworkCore;
using UCLoan.Data;
using UCLoan.Entities;

namespace UCLoan.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;
        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Set<User>().AddAsync(user);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
