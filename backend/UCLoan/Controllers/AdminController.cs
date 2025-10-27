using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UCLoan.Constants;
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
            var users = await _adminService.GetUsersAsync();

            return Ok(users);
        }
    }
}
