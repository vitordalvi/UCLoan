using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCLoan.DTOs.User;
using UCLoan.Services;

namespace UCLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var (success, message, user) = await _userService.GetMyProfileAsync();

            if (!User.Identity!.IsAuthenticated)
                return Unauthorized("Usuário não autenticado.");

            if (!success)
                return BadRequest(message);

            return Ok(new { success, message, user });
        }

        [HttpPost("update-my-data")]
        public async Task<IActionResult> UpdateMyData(UpdateMyDataDTO dto)
        {
            var (success, message) = await _userService.UpdateMyData(dto);

            if (!success)
                return BadRequest(message);

            return Ok(new { success, message });
        }
    }
}
