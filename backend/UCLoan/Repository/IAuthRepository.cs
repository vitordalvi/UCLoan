using UCLoan.Entities;

namespace UCLoan.Repository
{
    public interface IAuthRepository
    {
        // Adicionar um novo usuário
        Task AddUserAsync(User user);
        // Obter usuário por Id
        Task<User?> GetUserByIdAsync(Guid userId);
        // Obter usuário por email
        Task<User?> GetUserByEmailAsync(string email);
        // Obter usuário por CPF
        Task<User?> GetUserByCpfAsync(string CPF);
        // Salvar mudanças no banco de dados
        Task<bool> SaveChangesAsync();
    }
}
