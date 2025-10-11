using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCLoan.Constants;
using UCLoan.Extensions;
using UCLoan.Models;
using UCLoan.Services;
using UCLoan.ViewModels.Equipment;

namespace UCLoan.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EquipmentController : Controller
    {
        private readonly EquipmentService _equipmentService;

        public EquipmentController(EquipmentService equipmentService)
        {
            _equipmentService = equipmentService;
        }

        [HttpGet]
        public async Task<IActionResult> ManageEquipment(CancellationToken ct)
        {
            var equipments = await _equipmentService.GetAllEquipmentAsync(ct);

            // Gera dicionários de display names para os enums
            ViewBag.LoanStatusDisplay = EnumExtensions.GetDisplayNames<EquipmentConstants.EquipmentLoanStatus>();
            ViewBag.PhysicalStatusDisplay = EnumExtensions.GetDisplayNames<EquipmentConstants.EquipmentPhysicalStatus>();

            return View(equipments);
        }

        [HttpGet]
        public async Task<IActionResult> ManageEquipmentModels(CancellationToken ct)
        {
            var models = await _equipmentService.GetAllEquipmentModelsAsync();
            return View(models);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEquipment(int id, CancellationToken ct)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(ManageEquipmentModels));
            }

            var (success, error) = await _equipmentService.DeleteAsync(id);

            if (!success)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "Equipamento deletado com sucesso.";
            }

            return RedirectToAction(nameof(ManageEquipmentModels));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEquipmentModels(int id, CancellationToken ct)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(ManageEquipmentModels));
            }

            var (success, error) = await _equipmentService.DeleteEquipmentModelAsync(id);

            if (!success)
            {
                TempData["Error"] = error;
            } else
            {
                TempData["Success"] = "Modelo de equipamento deletado com sucesso.";
            }

            return RedirectToAction(nameof(ManageEquipmentModels));
        }

        [HttpGet]
        public async Task<IActionResult> AddEquipment()
        {
            await LoadDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEquipment(AddEquipmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync();
                return View(viewModel);
            }

            try
            {
                var (success, error) = await _equipmentService.CreateAsync(
                    viewModel.EquipmentModelId,
                    viewModel.EquipmentId,
                    viewModel.Description ?? string.Empty,
                    viewModel.PhysicalStatus);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, error ?? "Falha ao salvar o equipamento.");
                    await LoadDropdownsAsync();
                    return View(viewModel);
                }

                TempData["Success"] = "Equipamento adicionado com sucesso.";
                return RedirectToAction(nameof(ManageEquipment));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro inesperado: " + ex.Message);
                await LoadDropdownsAsync();
                return View(viewModel);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddEquipmentModel()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEquipmentModel([Bind("Name,Manufacturer")] Models.EquipmentModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _equipmentService.CreateModelAsync(model.Name, model.Manufacturer);

            return RedirectToAction(nameof(ManageEquipmentModels));
        }

        private async Task LoadDropdownsAsync()
        {
            ViewBag.AllEquipmentModels = await _equipmentService.GetAllEquipmentModelsSelectListAsync();
            ViewBag.AllLoanStatus = await _equipmentService.GetAllLoanStatusSelectListAsync();
            ViewBag.AllPhysicalStatus = await _equipmentService.GetAllPhysicalStatusSelectListAsync();
        }
    }
}