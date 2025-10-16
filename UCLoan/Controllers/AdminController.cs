using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UCLoan.Constants;
using UCLoan.Extensions;
using UCLoan.Models;
using UCLoan.Repositories;
using UCLoan.Services;
using UCLoan.ViewModels.Admin;

namespace UCLoan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AdminService _adminService;
        private readonly EquipmentService _equipmentService;
        private readonly LoanService _loanService;
        private readonly HomeService _homeService;

        public AdminController(AdminService adminService, EquipmentService equipmentService, LoanService loanService, HomeService homeService)
        {
            _adminService = adminService;
            _equipmentService = equipmentService;
            _loanService = loanService;
            _homeService = homeService;
        }

        // <-------------- INDEX (GET) -------------->

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var totalUsers = await _adminService.GetUsersAsync();
            var totalAdmins = await _adminService.GetUsersInRoleAsync("Admin");
            var newUsers = await _adminService.GetNewUsers();

            var allEquipments = await _equipmentService.GetAllEquipmentAsync();
            var loanedEquipments = await _loanService.GetAllLoansAsync();
            var maintanceEquipments = await _equipmentService.GetAllMaintanceEquipments();

            var totalLoans = await _loanService.GetAllLoanStatusAsync();
            var activeLoans = await _loanService.GetAllActiveLoans();
            var overdueLoans = await _loanService.GetAllOverdueLoans();


            var viewModel = new IndexViewModel
            {
                TotalUsers = totalUsers.Count,
                TotalAdmins = totalAdmins.Count,
                NewUsers = newUsers.Count,

                AllEquipments = allEquipments.Count,
                LoanedEquipments = loanedEquipments.Count,
                MaintanceEquipments = maintanceEquipments.Count,

                TotalLoans = totalLoans.Count,
                CurrentLoans = activeLoans.Count,
                LateLoans = overdueLoans.Count,
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> ManageQueue()
        {
            var queues = await _homeService.GetAllQueuesAsync();

            // Gera display names para os enums
            ViewBag.LoanStatusDisplay = EnumExtensions.GetDisplayNames<EquipmentConstants.EquipmentLoanStatus>();
            ViewBag.PhysicalStatusDisplay = EnumExtensions.GetDisplayNames<EquipmentConstants.EquipmentPhysicalStatus>();

            return View(queues);
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var users = await _adminService.GetUsersAsync();
            return View(users);
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