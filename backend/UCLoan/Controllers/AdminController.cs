using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCLoan.Constants;
using UCLoan.DTOs.Admin;
using UCLoan.Entities;
using UCLoan.Services;

namespace UCLoan.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AdminService _adminService;
        public AdminController(AdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("get-all-users")]
        public async Task<ActionResult<IList<User>>> GetUsersAsync()
        {
            var (success, message, users) = await _adminService.GetUsersAsync();

            return Ok(new {success, message, users});
        }

        [HttpPost("get-user-by-id")]
        public async Task<ActionResult<User?>> GetUserById([FromQuery] Guid userId)
        {
            var (success, message, user) = await _adminService.GetByIdAsync(userId);


            return Ok(new {success, message, user});
        }

        [HttpPost("get-user-by-email")]
        public async Task<ActionResult<User?>> GetByEmailAsync(string email)
        {
            var (success, message, user) = await _adminService.GetByEmailAsync(email);


            return Ok(new {success, message, user});
        }

        [HttpPost("update-user-data")]
        public async Task<ActionResult<User?>> UpdateUserData([FromQuery] Guid userId, [FromBody] UpdateDataDTO dto)
        {
            var (success, message, user) = await _adminService.UpdateUserData(userId, dto);

            return Ok(new { success, message, user});
        }
    }
}
