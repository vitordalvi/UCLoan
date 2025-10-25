using UCLoan.Entities;

namespace UCLoan.Repository
{
    public interface IAuthRepository
    {
        // Adicionar um novo usuário
        Task AddUserAsync(User user);
        // Obter usuário por Id
        Task<User?> GetByIdAsync(Guid userId);
        // Obter usuário por email
        Task<User?> GetByEmailAsync(string email);
        // Obter usuário por CPF
        Task<User?> GetByCpfAsync(string CPF);
        // Salvar mudanças no banco de dados
        Task<bool> SaveChangesAsync();
    }
}
