using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UCLoan.Constants;
using UCLoan.Models;
using UCLoan.Services;

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
            ViewBag.AllEquipmentModels = await _equipmentService.GetAllEquipmentModelsSelectListAsync();
            ViewBag.AllLoanStatus = await _equipmentService.GetAllLoanStatusSelectListAsync();
            ViewBag.AllPhysicalStatus = await _equipmentService.GetAllPhysicalStatusSelectListAsync();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEquipment([Bind("EquipmentModelId,EquipmentId,Description,PhysicalStatus")] Models.Equipment equipment)
        {
            var model = await _equipmentService.GetModel(equipment.EquipmentModel);

            if (equipment.EquipmentModelId <= 0)
            {
                ModelState.AddModelError(nameof(equipment.EquipmentModelId), "Modelo de equipamento inválido.");
                TempData["Error"] = "Modelo inválido.";
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Modelo de equipamento inválido.";
                await LoadDropdownsAsync();
                return View(equipment);
            }

            try
            {
                var (success, error) = await _equipmentService.CreateAsync(
                    equipment.EquipmentModel,
                    equipment.EquipmentId,
                    equipment.Description ?? string.Empty,
                    equipment.PhysicalStatus);

                if (!success)
                {
                    ModelState.AddModelError(string.Empty, error ?? "Falha ao salvar o equipamento.");
                    await LoadDropdownsAsync();
                    return View(equipment);
                }

                TempData["Success"] = "Equipamento adicionado com sucesso.";
                return RedirectToAction(nameof(ManageEquipment));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Erro inesperado: " + ex.Message);
                await LoadDropdownsAsync();
                return View(equipment);
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