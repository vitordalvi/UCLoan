using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
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

        public HomeController(ILogger<HomeController> logger, AdminService adminService, EquipmentService equipmentService, LoanService loanService)
        {
            _logger = logger;
            _adminService = adminService;
            _equipmentService = equipmentService;
            _loanService = loanService;
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
                EquipmentManufacturer = userEquipments.FirstOrDefault()?.EquipmentModel?.Manufacturer ?? "Nenhum equipamento emprestado",
                EquipmentName = userEquipments.FirstOrDefault()?.EquipmentModel?.Name ?? "Nenhum equipamento emprestado",
                EquipmentId = userEquipments.FirstOrDefault()?.EquipmentId ?? 0,
                EquipmentPhysicalStatus = userEquipments.FirstOrDefault()?.PhysicalStatus.GetDisplayName() ?? "Nenhum equipamento emprestado"
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
