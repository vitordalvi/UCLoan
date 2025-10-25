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

        // Adiciona um novo usuário no banco de dados
        public async Task AddUserAsync(User user)
        {
            await _context.Set<User>().AddAsync(user);
        }

        // Obtém um usuário pelo seu ID
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .FindAsync(userId);
        }

        // Obtém um usuário pelo seu email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // Obtem um usuário pelo seu CPF
        public async Task<User?> GetUserByCpfAsync(string CPF)
        {
            return await _context.Users
                .FirstOrDefaultAsync(c => c.CPF == CPF);
        }

        // Salva as mudanças no banco de dados
        // Se > 0, retorna True, entrando pra validação de sucesso
        public async Task<bool> SaveChangesAsync() =>
            await _context.SaveChangesAsync() > 0;
    }
}
