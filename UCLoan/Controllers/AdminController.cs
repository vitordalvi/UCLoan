using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UCLoan.Models;
using UCLoan.Services;

namespace UCLoan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(AdminService adminService, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _adminService = adminService;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Users()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _adminService.GetByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.AllRoles = await _adminService.GetAllRolesAsync();
            ViewBag.UserRoles = await _adminService.GetUserRolesAsync(user);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser([Bind("Id,FullName,CPF,Email,PhoneNumber")] ApplicationUser model, string[]? roles)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllRoles = await _adminService.GetAllRolesAsync();
                ViewBag.UserRoles = roles ?? Array.Empty<string>();
                return View(model);
            }

            var (success, errors) = await _adminService.UpdateDataAsync(model, roles ?? Array.Empty<string>());

            if (!success)
            {
                foreach (var e in errors)
                {
                    ModelState.AddModelError(string.Empty, e);
                }

                ViewBag.AllRoles = await _adminService.GetAllRolesAsync();
                ViewBag.UserRoles = roles ?? Array.Empty<string>();
                return View(model);
            }

            TempData["Success"] = "Dados do usuário atualizados com sucesso!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "Usuário não encontrado.";
                return RedirectToAction(nameof(Users));
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(" | ", result.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["Success"] = "Usuário excluído com sucesso.";
            }

            return RedirectToAction(nameof(Users));
        }
    }
}