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

        // <-------------- EQUIPAMENTOS (GET) -------------->

        [HttpGet]
        public async Task<IActionResult> ManageEquipment(CancellationToken ct)
        {
            var equipments = await _equipmentService.GetAllEquipmentAsync(ct);

            // Gera display names para os enums
            ViewBag.LoanStatusDisplay = EnumExtensions.GetDisplayNames<EquipmentConstants.EquipmentLoanStatus>();
            ViewBag.PhysicalStatusDisplay = EnumExtensions.GetDisplayNames<EquipmentConstants.EquipmentPhysicalStatus>();

            return View(equipments);
        }

        [HttpGet]
        public async Task<IActionResult> AddEquipment()
        {
            await LoadDropdownsAsync();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditEquipment(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(ManageEquipment));
            }

            var equipment = await _equipmentService.GetByIdAsync(id);

            if (equipment == null)
            {
                TempData["Error"] = "Equipamento não encontrado";
                return RedirectToAction(nameof(ManageEquipment));
            }

            await LoadDropdownsAsync();
            return View(equipment);
        }

        // <-------------- EQUIPAMENTOS (POST) -------------->

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEquipment(int id, CancellationToken ct)
        {
            if (id <= 0)
            {
                TempData["Error"] = "Id inválido.";
                return RedirectToAction(nameof(ManageEquipment));
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

            return RedirectToAction(nameof(ManageEquipment));
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEquipment(EditEquipmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Algo não está valido.";

                ModelState.AddModelError(string.Empty, "Dados inválidos. Verifique os campos.");
                await LoadDropdownsAsync();
                return View(viewModel);
            }

            var (success, error) = await _equipmentService.UpdateAsync(
                viewModel.Id,
                viewModel.EquipmentId,
                viewModel.Description ?? string.Empty,
                viewModel.PhysicalStatus,
                viewModel.EquipmentModelId);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, error ?? "Falha ao atualizar o equipamento.");
                TempData["Error"] = "Erro ao salvar.";
                await LoadDropdownsAsync();
                return View(viewModel);
            }

            TempData["Success"] = "Equipamento atualizado com sucesso.";
            return RedirectToAction(nameof(ManageEquipment));
        }

        // <-------------- MODELO EQUIPAMENTO (GET) -------------->

        [HttpGet] 
        public async Task<IActionResult> ManageEquipmentModels(CancellationToken ct)
        {
            var models = await _equipmentService.GetAllEquipmentModelsAsync();
            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> AddModel()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditModel(int id)
        {
            if (id <= 0) return NotFound();

            var model = await _equipmentService.GetModelByIdAsync(id);

            if (model == null) return NotFound();

            await LoadDropdownsAsync();
            return View(model);
        }

        // <-------------- MODELO EQUIPAMENTO (POST) -------------->

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteModel(int id, CancellationToken ct)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddModel([Bind("Name,Manufacturer")] AddModelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _equipmentService.CreateModelAsync(model.Name, model.Manufacturer);

            return RedirectToAction(nameof(ManageEquipmentModels));
        }

        [HttpPost]
        public async Task<IActionResult> EditModel([Bind("Id,Name,Manufacturer")] EditModelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, error) = await _equipmentService.UpdateModelAsync(model.Id, model.Name, model.Manufacturer);

            if (!success)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "Modelo de equipamento deletado com sucesso.";
            }
            

            return RedirectToAction(nameof(ManageEquipmentModels));
        }

        // <-------------- UTILS -------------->

        private async Task LoadDropdownsAsync()
        {
            ViewBag.AllEquipmentModels = await _equipmentService.GetAllEquipmentModelsSelectListAsync();
            ViewBag.AllLoanStatus = await _equipmentService.GetAllLoanStatusSelectListAsync();
            ViewBag.AllPhysicalStatus = await _equipmentService.GetAllPhysicalStatusSelectListAsync();
        }
    }
}