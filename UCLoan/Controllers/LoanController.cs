using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using UCLoan.Constants;
using UCLoan.Extensions;
using UCLoan.Services;
using UCLoan.ViewModels.Loan;
using System.Linq;
using UCLoan.ViewModels.Home;
using System.Text.Json;

namespace UCLoan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LoanController : Controller
    {
        private readonly LoanService _loanService;
        private readonly AdminService _adminService;
        private readonly EquipmentService _equipmentService;
        private readonly HomeService _homeService;

        public LoanController(LoanService loanService, AdminService adminService, EquipmentService equipmentService, HomeService homeService) {
            _loanService = loanService;
            _adminService = adminService;
            _equipmentService = equipmentService;
            _homeService = homeService;
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
            await LoadDropdownsAsync();
            return View();
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

        [HttpGet]
        public async Task<IActionResult> TakeOutRequest(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction("Admin", "ManageQueue");
            }

            var queue = await _homeService.GetQueueByIdAsync(id);

            if (queue == null)
            {
                TempData["Error"] = "Solicitação não encontrada.";
                return RedirectToAction("Admin", "ManageQueue");
            }

            await LoadDropdownsAsync();

            var viewModel = new TakeOutRequestViewModel
            {
                QueueId = queue.Id,
                User = queue.User,
                UserEmail = queue.User?.Email ?? string.Empty,
                RequestedAt = queue.CreatedAt,
                EndDate = queue.ReturnDate ?? DateTime.Today.AddDays(7),
                Description = queue.Description
            };

            return View(viewModel);
        }

        // <-------------- EMPRÉSTIMOS (POST) -------------->

        // Adicionar Empréstimo 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLoan([Bind("UserEmail,EquipmentId,StartDate,EndDate,Description")] AddLoanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(model);
            }

            if (!model.EndDate.HasValue)
            {
                ModelState.AddModelError("EndDate", "A data de devolução é obrigatória.");

                await LoadDropdownsAsync();
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
        public async Task<IActionResult> ConfirmReceipt(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(ManageLoans));
            }

            var (success, error) = await _loanService.ConfirmReceiptAsync(id);

            if (!success)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "Recebimento confirmado com sucesso.";
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeOutRequest([Bind("QueueId,UserEmail,EquipmentId,RequestedAt,EndDate,Description")] TakeOutRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Algo não está valido.";
                ModelState.AddModelError(string.Empty, "Dados inválidos. Verifique os campos.");
                await LoadDropdownsAsync();
                return View(model);
            }

            var email = model.UserEmail?.Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError(nameof(model.UserEmail), "O e-mail do recebedor é obrigatório.");
                await LoadDropdownsAsync();
                return View(model);
            }

            var requestedAt = model.RequestedAt == default ? DateTime.Now : model.RequestedAt;

            var (success, error) = await _loanService.CreateAsync(
                email,
                model.EquipmentId,
                requestedAt,
                model.EndDate,
                model.Description);

            if (!success)
            {
                TempData["Error"] = error;
                await LoadDropdownsAsync();
                return View(model);
            }

            TempData["Success"] = "Empréstimo realizado com sucesso!";
            return RedirectToAction("ManageQueue", "Admin");
        }

        // <-------------- LOANS (UTILS) -------------->

        // Carrega as listas suspensas
        private async Task LoadDropdownsAsync()
        {
            ViewBag.AllAvailableEquipments = await _loanService.GetAllEquipmentsAvailableSelectListAsync();
            ViewBag.AllLoanStatus = await _equipmentService.GetAllLoanStatusSelectListAsync();
            ViewBag.AllPhysicalStatus = await _equipmentService.GetAllPhysicalStatusSelectListAsync();
        }
    }
}
