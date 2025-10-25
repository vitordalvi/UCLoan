using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UCLoan.DTOs.Authentication;
using UCLoan.Entities;
using UCLoan.Repository;

namespace UCLoan.Services
{
    public class AuthService : BaseService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        public AuthService(IAuthRepository authRepository, LogService logService, IConfiguration configuration) : base(logService)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        // Obter usuário por email
        public Task<User?> GetUserByEmailAsync(string email) =>
            _authRepository.GetByEmailAsync(email);

        // Obter usuário por Id
        public Task<User?> GetUserByIdAsync(Guid userId) =>
            _authRepository.GetByIdAsync(userId);
        
        // Obter usuário por CPF
        public Task<User?> GetUserByCpfAsync(string CPF) =>
            _authRepository.GetByCpfAsync(CPF);


        // Criar o usuário
        public async Task<(bool Success, string Message)> RegisterAsync(UserRegisterDTO request)
        {
            if (await _authRepository.GetByEmailAsync(request.Email) != null)
            {
                return (false, "O e-mail já está em uso.");
            }

            if (await _authRepository.GetByCpfAsync(request.CPF) != null)
            {
                return (false, "O CPF já está em uso.");
            }

            var user = new User();

            user.Email = request.Email;
            user.PasswordHash = HashPassword(user, request.Password);
            user.Name = request.Name;
            user.CPF = request.CPF;
            user.CreatedAt = DateTime.UtcNow;


            await _authRepository.AddUserAsync(user);
            var saved = await _authRepository.SaveChangesAsync();

            if (saved)
            {
                // Log de registro de usuários
                await _logService.LogAsync(
                    user.Id,
                    "User Registered",
                    $"{user.Email}",
                    user.Id,
                    new { user.Email, user.Name, user.CPF });
            }

            return saved ? (true, "Usuário registrado com sucesso.") : (false, "Erro ao registrar usuário.");
        }

        // Login do usuário
        public async Task<(bool Success, string Message, TokenResponseDTO? Token)> LoginAsync(UserLoginDTO request)
        {
            var user = await _authRepository.GetByEmailAsync(request.Email.ToLower());

            if (user == null)
                return (false, "O usuário não foi encontrado", null);

            if (!VerifyPassword(user, request.Password))
                return (false, "Senha incorreta.", null);

            var tokenResult = await CreateTokenResponse(user);

            // Gera uma log da ação de login
            await _logService.LogAsync(
                user.Id,
                "Usuário efetuou login",
                $"{user.Email.ToLower()}",
                user.Id,
                new { user.Email, user.Name });

            return (tokenResult.Success, "Autenticação efetuada.", tokenResult.Token);
        }

        // Atualiza os tokens (Access Token e Refresh Token)
        public async Task<(bool Success, string Message, TokenResponseDTO? Token)> RefreshTokensAsync(RefreshTokenRequestDTO request)
        {
            var (valid, message, user) = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);

            // Se a validação falhar ou não houver usuário, retornar o erro apropriado
            if (!valid || user == null)
            {
                return (false, message, null);
            }

            // Revoga o antigo e gera um novo par de tokens
            var tokenResult = await CreateTokenResponse(user);

            // Atualiza o Refresh Token e salva no banco
            user.RefreshToken = tokenResult.Token.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            // Cria novo par de tokens e devolve com os parametros do resultado
            var saved = await _authRepository.SaveChangesAsync();
            return saved ? (true, "Tokens atualizados com sucesso.", tokenResult.Token) : (false, "Houve um erro ao atualizar os tokens", null);
        }

        // Faz a validação do Refresh Token
        private async Task<(bool Success, string Message, User? user)> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await _authRepository.GetByIdAsync(userId);

            if (user == null || user.RefreshToken != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return (false, "O usuário não existe.", null);
            }

            return (true, string.Empty, user);
        }

        // Gera e salva o Refresh Token no banco de dados
        private async Task<(bool Success, string Error, string token)> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh Token vai expirar em 7 dias

            var saved = await _authRepository.SaveChangesAsync();

            return saved ? (true, "Refresh Token gerado e salvo com sucesso.", refreshToken) : (false, "Houve um erro ao gerar ou salvar os tokens", string.Empty);
        }

        // Cria o Token de Acesso e Refresh Token
        private async Task<(bool Success, string Message, TokenResponseDTO Token)> CreateTokenResponse(User user)
        {
            var (success, message, token) = await GenerateAndSaveRefreshTokenAsync(user);

            var tokenResponse = new TokenResponseDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = token
            };

            // Sempre retorna sucesso, pois a validação já foi feita antes
            return (true, string.Empty, tokenResponse);
        }

        // Cria a senha hash
        private string HashPassword(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.HashPassword(user, password);
        }

        // Cria a senha hash
        private bool VerifyPassword(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

            return result == PasswordVerificationResult.Success;
        }

        // Gera o Refresh Token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            // Converte o array de bytes para uma string Base64
            return Convert.ToBase64String(randomNumber);
        }

        // Cria o Token de Acesso JWT
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
