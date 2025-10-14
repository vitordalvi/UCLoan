using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
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
        public async Task<IActionResult> ManageLoans()
        {
            var loans = await _loanService.GetAllLoansAsync();

            return View(loans);
        }

        [HttpGet]
        public async Task<IActionResult> AddLoan()
        {
            var viewModel = new AddLoanViewModel
            {
                AvailableEquipments = await _loanService.GetAllEquipmentsAvailableSelectListAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLoan([Bind("UserEmail,EquipmentId,SelectedEquipmentId,AvailableEquipments,LoanDate,ReturnDate,Description")] AddLoanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.ReturnDate.HasValue)
            {
                ModelState.AddModelError("ReturnDate", "A data de devolução é obrigatória.");
                model.AvailableEquipments = await _loanService.GetAllEquipmentsAvailableSelectListAsync();
                return View(model);
            }

            var (success, error) = await _loanService.CreateAsync(
                model.UserEmail,
                model.EquipmentId,
                DateTime.Now,
                model.ReturnDate.Value,
                model.Description);

            return RedirectToAction(nameof(ManageLoans));
        }

        private async Task LoadDropdownsAsync()
        {
            ViewBag.AllAvailableEquipments = await _loanService.GetAllEquipmentsAvailableSelectListAsync();
        }
    }
}
