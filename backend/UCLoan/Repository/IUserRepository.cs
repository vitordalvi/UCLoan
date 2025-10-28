using UCLoan.Entities;

namespace UCLoan.Repository
{
    public interface IUserRepository
    {
        // Método para pegar o ID do usuário logado a partir do token JWT
        Task<Guid> GetCurrentUserId();

        // Obtém um usuário pelo seu ID
        Task<User?> GetByIdAsync(Guid userId);

        // Obtém um usuário pelo seu email
        Task<User?> GetByEmailAsync(string email);

        // Obtém um usuário pelo seu CPF
        Task<User?> GetByCpfAsync(string CPF);

        // Verifica se o usuário existe pelo seu ID
        Task<bool> IsUserExists(Guid userId);

        // Verifica se o e-mail já está em uso
        Task<bool> IsEmailInUse(string email);

        // Verifica se o CPF já está em uso
        Task<bool> IsCpfInUse(string cpf);

        // Lista todos os usuários
        Task<IList<User>> GetUsersAsync();

        // Atualiza os dados do usuário
        Task Update(User user);

        // Salva os dados no banco
        Task<bool> SaveChangesAsync();
    }
}
