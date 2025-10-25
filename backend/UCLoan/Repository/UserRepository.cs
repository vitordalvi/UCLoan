using Microsoft.EntityFrameworkCore;
using UCLoan.Data;
using UCLoan.Entities;

namespace UCLoan.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // Obtém um usuário pelo seu ID
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // Obtém um usuário pelo seu email
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // Obtem um usuário pelo seu CPF
        public async Task<User?> GetByCpfAsync(string CPF)
        {
            return await _context.Users
                .FirstOrDefaultAsync(c => c.CPF == CPF);
        }
        
        // Verifica se o usuário existe pelo seu ID
        public async Task<bool> IsUserExists(Guid userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        // Atualiza os dados do usuário
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }
        // Verifica se o email já está em uso
        public async Task<bool> IsEmailInUse(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Salva os dados no banco
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
