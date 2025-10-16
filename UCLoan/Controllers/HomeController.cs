using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Versioning;
using UCLoan.Extensions;
using UCLoan.Models;
using UCLoan.Services;
using UCLoan.ViewModels.Home;

namespace UCLoan.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AdminService _adminService;
        private readonly EquipmentService _equipmentService;
        private readonly LoanService _loanService;
        private readonly HomeService _homeService;

        public HomeController(ILogger<HomeController> logger, AdminService adminService, EquipmentService equipmentService, LoanService loanService, HomeService homeService)
        {
            _logger = logger;
            _adminService = adminService;
            _equipmentService = equipmentService;
            _loanService = loanService;
            _homeService = homeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            var user = await _adminService.GetByIdAsync(userId);

            if (user == null)
            {
                return Forbid();
            }

            var userRole = await _adminService.GetUserRolesAsync(user);
            var userLoans = await _loanService.GetUserLoansByEmailAsync(user.Email!);
            var userEquipments = await _loanService.GetUserLoansEquipmentByEmailAsync(user.Email!);

            var viewModel = new IndexViewModel
            {
                UserName = user.FullName,
                UserEmail = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                CPF = user.CPF,
                Role = userRole.FirstOrDefault()!,
                ActiveLoans = userLoans.Count,
                LoanId = userLoans.FirstOrDefault()?.Id ?? 0,
                LoanStartDate = userLoans.FirstOrDefault()?.StartDate,
                LoanEndDate = userLoans.FirstOrDefault()?.EndDate,
                EquipmentManufacturer = userEquipments.FirstOrDefault()?.EquipmentModel?.Manufacturer ?? "N/A",
                EquipmentName = userEquipments.FirstOrDefault()?.EquipmentModel?.Name ?? "",
                EquipmentId = userEquipments.FirstOrDefault()?.EquipmentId ?? 0,
                EquipmentPhysicalStatus = userEquipments.FirstOrDefault()?.PhysicalStatus.GetDisplayName() ?? "N/A"
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Queue()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Home");
            }

            var user = await _adminService.GetByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var viewModel = new QueueViewModel
            {
                UserEmail = user.Email!,
            };

            return View(viewModel);
        }

        // <-------------- QUEUE (POST) -------------->

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Queue([Bind("User,Description,CreatedAt")]QueueViewModel model)
        {
            var user = await _adminService.GetByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Algo não está valido.";
                return View(model);
            }

            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            if (user.Email == null)
            {
                TempData["Error"] = "Usuário não possui email cadastrado.";
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            model.UserEmail = user.Email!;

            var (success, error) = await _homeService.CreateQueueAsync(
                model.UserEmail,
                model.Description,
                model.CreatedAt);

            if (!success)
            {
                TempData["Error"] = error;
                return View(model);
            }

            TempData["Success"] = error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(Queue));
            }

            var (success, error) = await _homeService.DeleteQueueAsync(id);

            if (!success)
            {
                TempData["Error"] = error;
                return RedirectToAction("ManageQueue", "Admin");
            }


            TempData["Success"] = error;
            return RedirectToAction("ManageQueue", "Admin");
        }
    
        // <-------------- UTILS -------------->

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
