using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCLoan.DTOs.Authentication;
using UCLoan.Services;

namespace UCLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        // Endpoint de registro
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO request)
        {
            var (success, message) = await _authService.RegisterAsync(request);

            if (!success)
                return BadRequest(message);

            return Ok(message);
        }

        // Endpoint de login
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login(UserLoginDTO request)
        {
            var (success, message, token) = await _authService.LoginAsync(request);

            if (!success)
                return BadRequest(message);

            return Ok(token);
        }

        // Endpoint para dar refresh no token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO request)
        {
            var (success, message, token) = await _authService.RefreshTokensAsync(request);

            if (!success)
                return BadRequest(message);

            return Ok(token);
        }

        // Endpoint para somente usuários autenticados
        [Authorize]
        [HttpGet("authenticated-only")]
        public IActionResult AuthenticatedOnlyEndpoint()
        {
            return Ok("Você está autenticado.");
        }

        // Outros cargos podem ser adicionados, só mudar a annotation, exemplo: (Roles = "Admin, Manager")
        [Authorize(Roles = "Admin,")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok("Você está autenticado como Administrador.");
        }
    }
}
