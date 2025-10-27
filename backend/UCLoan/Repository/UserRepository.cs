using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UCLoan.Data;
using UCLoan.Entities;

namespace UCLoan.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserRepository(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // Método para pegar o ID do usuário logado a partir do token JWT
        public async Task<Guid> GetCurrentUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var idClaim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(idClaim, out var userId))
            {
                return await Task.FromResult(userId);
            }

            return Guid.Empty;
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

        // Verifica se o email já está em uso
        public async Task<bool> IsEmailInUse(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Lista todos os usuários
        public async Task<IList<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Atualiza os dados do usuário
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
        }

        // Salva os dados no banco
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
