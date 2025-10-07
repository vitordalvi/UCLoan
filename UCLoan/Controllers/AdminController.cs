using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCLoan.Models;
using UCLoan.Repositories;
using UCLoan.Services;

namespace UCLoan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly IAdminRepository _adminRepository;

        public AdminController(AdminService adminService, IAdminRepository adminRepository)
        {
            _adminService = adminService;
            _adminRepository = adminRepository;
        }

        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetUsersAsync();
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
        public async Task<IActionResult> EditUser([Bind("Id,FullName,CPF,Email,PhoneNumber")] ApplicationUser model, string? role)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AllRoles = await _adminService.GetAllRolesAsync();
                ViewBag.UserRoles = string.IsNullOrWhiteSpace(role) ? Array.Empty<string>() : new[] { role };
                return View(model);
            }

            var rolesArray = string.IsNullOrWhiteSpace(role) ? Array.Empty<string>() : new[] { role };
            var (success, errors) = await _adminService.UpdateDataAsync(model, rolesArray);

            if (!success)
            {
                foreach (var e in errors)
                {
                    ModelState.AddModelError(string.Empty, e);
                }

                ViewBag.AllRoles = await _adminService.GetAllRolesAsync();
                ViewBag.UserRoles = rolesArray;
                return View(model);
            }

            TempData["Success"] = "Dados do usuário atualizados com sucesso!";
            return RedirectToAction(nameof(Users));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            // Claim para identificar o ID do usuário autenticado
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(Users));
            }

            var (success, error) = await _adminService.DeleteUserAsync(id, currentUserId);
            if (!success)
                TempData["Error"] = error;
            else
                TempData["Success"] = "Usuário removido com sucesso.";

            return RedirectToAction(nameof(Users));
        }
    }
}