using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using UCLoan.Constants;
using UCLoan.Extensions;
using UCLoan.Services;
using UCLoan.ViewModels.Loan;
using System.Linq;

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

        // <-------------- EMPRÉSTIMOS (GET) -------------->

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ManageLoans()
        {
            var loans = await _loanService.GetAllLoansAsync();
            ViewBag.LoanStatusDisplay = EnumExtensions.GetDisplayNames<EquipmentConstants.EquipmentLoanStatus>();

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

        [HttpGet]
        public async Task<IActionResult> EditLoan(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(ManageLoans));
            }

            var loan = await _loanService.GetByIdAsync(id);

            if (loan == null)
            {
                TempData["Error"] = "Empréstimo não encontrado.";
                return RedirectToAction(nameof(ManageLoans));
            }

            var viewModel = new EditLoanViewModel
            {
                Id = loan.Id,
                EquipmentId = loan.Equipment.EquipmentId,
                LoanStatus = loan.Equipment.LoanStatus,
                PhysicalStatus = loan.Equipment.PhysicalStatus,
                StartDate = loan.StartDate,
                EndDate = loan.EndDate,
                Description = loan.Description
            };

            await LoadDropdownsAsync();
            return View(viewModel);
        }

        // <-------------- EMPRÉSTIMOS (POST) -------------->

        // Adicionar Empréstimo 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLoan([Bind("UserEmail,EquipmentId,SelectedEquipmentId,AvailableEquipments,StartDate,EndDate,Description")] AddLoanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.EndDate.HasValue)
            {
                ModelState.AddModelError("EndDate", "A data de devolução é obrigatória.");
                model.AvailableEquipments = await _loanService.GetAllEquipmentsAvailableSelectListAsync();
                return View(model);
            }

            var (success, error) = await _loanService.CreateAsync(
                model.UserEmail,
                model.EquipmentId,
                DateTime.Now,
                model.EndDate.Value,
                model.Description);

            return RedirectToAction(nameof(ManageLoans));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLoan([Bind("Id,EquipmentId,LoanStatus,PhysicalStatus,StartDate,EndDate,Description")] EditLoanViewModel model)
        {
            if (model.EquipmentId <= 0)
            {
                TempData["Error"] = "O Id não foi informado.";

                ModelState.AddModelError(string.Empty, "Dados inválidos. Verifique os campos.");
                await LoadDropdownsAsync();
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Algo não está valido.";

                ModelState.AddModelError(string.Empty, "Dados inválidos. Verifique os campos.");
                await LoadDropdownsAsync();
                return View(model);
            }

            var (success, error) = await _loanService.UpdateAsync(
                model.Id,
                model.EquipmentId,
                model.LoanStatus,
                model.PhysicalStatus,
                model.StartDate,
                model.EndDate!.Value,
                model.Description);

            if (!success)
            {
                TempData["Error"] = error;
                await LoadDropdownsAsync();
                return View(model);
            }

            TempData["Success"] = "Empréstimo atualizado com sucesso.";
            return RedirectToAction(nameof(ManageLoans));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(ManageLoans));
            }

            var (success, error) = await _loanService.DeleteAsync(id);

            if (!success)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "Empréstimo deletado com sucesso.";
            }

            return RedirectToAction(nameof(ManageLoans));
        }

        // <-------------- LOANS (UTILS) -------------->

        // Carrega as listas suspensas necessárias (exceto equipamentos se sobrescrever for false)
        private async Task LoadDropdownsAsync()
        {
            ViewBag.AllAvailableEquipments = await _loanService.GetAllEquipmentsAvailableSelectListAsync();
            ViewBag.AllLoanStatus = await _equipmentService.GetAllLoanStatusSelectListAsync();
            ViewBag.AllPhysicalStatus = await _equipmentService.GetAllPhysicalStatusSelectListAsync();
        }
    }
}
