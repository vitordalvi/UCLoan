using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCLoan.Services;
using UCLoan.ViewModels.Loan;

namespace UCLoan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LoanController : Controller
    {
        private readonly LoanService _loanService;
        private readonly AdminService _adminService;
        private readonly EquipmentService _equipmentService;

        public LoanController(LoanService loanService, AdminService adminService, EquipmentService equipmentService) {
            _loanService = loanService;
            _adminService = adminService;
            _equipmentService = equipmentService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ManageLoans()
        {
            var loans = _loanService.GetAllLoansAsync();

            return View(loans);
        }

        private async Task LoadDropdownsAsync()
        {
            ViewBag.AllAvailableEquipments = await _equipmentService.GetAllLoanStatusSelectListAsync();
            ViewBag.AllEquipmentModels = await _equipmentService.GetAllEquipmentModelsSelectListAsync();
            ViewBag.AllLoanStatus = await _equipmentService.GetAllLoanStatusSelectListAsync();
            ViewBag.AllPhysicalStatus = await _equipmentService.GetAllPhysicalStatusSelectListAsync();
        }
    }
}
