using Microsoft.AspNetCore.Mvc.Rendering;
using UCLoan.Constants;
using UCLoan.Extensions;
using UCLoan.Models;
using UCLoan.Repositories;

namespace UCLoan.Services
{
    public class EquipmentService
    {
        private readonly IEquipmentRepository _equipmentRepository;

        public EquipmentService(IEquipmentRepository equipmentRepository)
        {
            _equipmentRepository = equipmentRepository;
        }

        public Task<List<Equipment>> GetAllEquipmentAsync(CancellationToken ct = default) =>
            _equipmentRepository.GetAllEquipmentAsync(ct);

        public Task<List<EquipmentModel>> GetAllEquipmentModelsAsync(CancellationToken ct = default) =>
            _equipmentRepository.GetAllEquipmentModelsAsync(ct);
        public Task<List<EquipmentConstants.EquipmentPhysicalStatus>> GetAllEquipmentPhysicalStatus(CancellationToken ct = default) =>
            _equipmentRepository.GetAllEquipmentPhysicalStatus(ct);
        public Task<List<EquipmentConstants.EquipmentLoanStatus>> GetAllEquipmentLoanStatus(CancellationToken ct = default) =>
            _equipmentRepository.GetAllEquipmentLoanStatus(ct);

        public Task<EquipmentModel?> GetModelOfEquipment(Equipment equipment, CancellationToken ct = default) =>
            _equipmentRepository.GetModelOfEquipment(equipment, ct);

        public async Task<List<SelectListItem>> GetAllEquipmentModelsSelectListAsync()
        {
            var allModels = await _equipmentRepository.GetAllEquipmentModelsAsync();

            return allModels
                .Select(m => new SelectListItem
                {
                    Text = m.Name,
                    Value = m.Id.ToString()
                })
                .ToList();
        }

        public Task<List<SelectListItem>> GetAllPhysicalStatusSelectListAsync()
        {
            var list = Enum.GetValues(typeof(EquipmentConstants.EquipmentPhysicalStatus))
                .Cast<EquipmentConstants.EquipmentPhysicalStatus>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString()
                })
                .ToList();

            return Task.FromResult(list);
        }

        public Task<List<SelectListItem>> GetAllLoanStatusSelectListAsync()
        {
            var list = Enum.GetValues(typeof(EquipmentConstants.EquipmentLoanStatus))
                .Cast<EquipmentConstants.EquipmentLoanStatus>()
                .Select(e => new SelectListItem
                {
                    Text = e.GetDisplayName(),
                    Value = e.ToString()
                })
                .ToList();

            return Task.FromResult(list);
        }

        public Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _equipmentRepository.GetByIdAsync(id, ct);

        public async Task<(bool Success, string? Error)> CreateAsync(
            int equipmentModelId,
            int equipmentId,
            string description,
            EquipmentConstants.EquipmentPhysicalStatus physicalStatus,
            CancellationToken ct = default)
        {
            if (equipmentModelId <= 0)
            {
                return (false, "O modelo escolhido não existe.");
            }

            var model = await _equipmentRepository.GetModelByIdAsync(equipmentModelId);

            if (model == null)
            {
                return (false, "Modelo inválido");
            }

            var equipment = new Equipment
            {
                EquipmentId = equipmentId,
                LoanStatus = EquipmentConstants.EquipmentLoanStatus.Available,
                PhysicalStatus = physicalStatus,
                Description = description.Trim(),
                EquipmentModelId = equipmentModelId,
                EquipmentModel = model
            };

            await _equipmentRepository.AddAsync(equipment, ct);
            var saved = await _equipmentRepository.SaveChangesAsync(ct);

            return saved ? (true, "Equipamento salvo.") : (false, "Falha ao salvar equipamento.");
        }

        public async Task<(bool Success, string? Error)> UpdateAsync(
            int id,
            int equipmentId,
            string description,
            EquipmentConstants.EquipmentPhysicalStatus physicalStatus,
            EquipmentConstants.EquipmentLoanStatus loanStatus,
            int equipmentModelId,
            CancellationToken ct = default)
        {
            var entity = await _equipmentRepository.GetByIdAsync(id, ct);
            if (entity == null)
                return (false, "Equipamento não encontrado.");

            var model = await _equipmentRepository.GetModelByIdAsync(equipmentModelId, ct);
            if (model == null)
                return (false, "Modelo informado não existe.");

            entity.EquipmentId = equipmentId;
            entity.Description = description?.Trim() ?? string.Empty;
            entity.PhysicalStatus = physicalStatus;
            entity.LoanStatus = loanStatus;
            entity.EquipmentModel = model;

            await _equipmentRepository.UpdateAsync(entity, ct);
            var saved = await _equipmentRepository.SaveChangesAsync(ct);
            return saved ? (true, null) : (false, "Nenhuma alteração persistida.");
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int id, CancellationToken ct = default)
        {
            var entity = await _equipmentRepository.GetByIdAsync(id, ct);
            if (entity == null)
                return (false, "Equipamento não encontrado.");

            if (entity.LoanStatus is EquipmentConstants.EquipmentLoanStatus.Borrowed
                or EquipmentConstants.EquipmentLoanStatus.Overdue)
            {
                return (false, "Não é possível excluir equipamento emprestado ou em atraso.");
            }

            await _equipmentRepository.DeleteAsync(entity, ct);
            var saved = await _equipmentRepository.SaveChangesAsync(ct);
            return saved ? (true, null) : (false, "Falha ao excluir equipamento.");
        }

        public async Task<(bool Success, string? Error)> ChangeLoanStatusAsync(
            int id,
            EquipmentConstants.EquipmentLoanStatus newStatus,
            CancellationToken ct = default)
        {
            var entity = await _equipmentRepository.GetByIdAsync(id, ct);
            if (entity == null)
                return (false, "Equipamento não encontrado.");

            // Não voltar de Returned para emprestado direto
            if (entity.LoanStatus == EquipmentConstants.EquipmentLoanStatus.Returned &&
                newStatus == EquipmentConstants.EquipmentLoanStatus.Borrowed)
            {
                return (false, "Transição de 'Devolvido' para 'Emprestado' não permitida.");
            }

            entity.LoanStatus = newStatus;
            await _equipmentRepository.UpdateAsync(entity, ct);
            var saved = await _equipmentRepository.SaveChangesAsync(ct);
            return saved ? (true, null) : (false, "Falha ao atualizar status de empréstimo do equipamento.");
        }

        public async Task<(bool Success, string? Error)> ChangePhysicalStatusAsync(
            int id,
            EquipmentConstants.EquipmentPhysicalStatus newStatus,
            CancellationToken ct = default)
        {
            var entity = await _equipmentRepository.GetByIdAsync(id, ct);
            if (entity == null)
                return (false, "Equipamento não encontrado.");

            entity.PhysicalStatus = newStatus;
            await _equipmentRepository.UpdateAsync(entity, ct);
            var saved = await _equipmentRepository.SaveChangesAsync(ct);
            return saved ? (true, null) : (false, "Falha ao atualizar estado físico do equipamento.");
        }

        public async Task<(bool Success, string? Error)> CreateModelAsync(
            string name,
            string manufacturer,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(name))
                return (false, "Nome do modelo é obrigatório.");
            if (string.IsNullOrWhiteSpace(manufacturer))
                return (false, "Fabricante é obrigatório.");

            var model = new EquipmentModel
            {
                Name = name.Trim(),
                Manufacturer = manufacturer.Trim()
            };

            await _equipmentRepository.AddModelAsync(model, ct);
            var saved = await _equipmentRepository.SaveChangesAsync(ct);
            return saved ? (true, null) : (false, "Falha ao salvar modelo.");
        }

        public async Task<(bool Success, string? Error)> UpdateModelAsync(
            int id,
            string name,
            string manufacturer,
            CancellationToken ct = default)
        {
            var model = await _equipmentRepository.GetModelByIdAsync(id, ct);
            if (model == null)
                return (false, "Modelo não encontrado.");

            if (string.IsNullOrWhiteSpace(name))
                return (false, "Nome do modelo é obrigatório.");

            if (string.IsNullOrWhiteSpace(manufacturer))
                return (false, "Fabricante é obrigatório.");

            model.Name = name.Trim();
            model.Manufacturer = manufacturer.Trim();

            await _equipmentRepository.UpdateModelAsync(model, ct);
            var saved = await _equipmentRepository.SaveChangesAsync(ct);
            return saved ? (true, null) : (false, "Nenhuma alteração foi confirmada.");
        }

        public Task<EquipmentModel?> GetModelByIdAsync(int id, CancellationToken ct = default) =>
                        _equipmentRepository.GetModelByIdAsync(id, ct);

        public async Task<(bool Success, string? Error)> DeleteEquipmentModelAsync(int id, CancellationToken ct = default)
        {
            if (id <= 0)
            {
                return (false, "Id inválido.");
            }

            var model = await _equipmentRepository.GetModelByIdAsync(id, ct);

            if (model == null)
            {
                return (false, "Modelo não encontrado.");
            }

            await _equipmentRepository.DeleteEquipmentModelAsync(model, ct);
            var saved = await _equipmentRepository.SaveChangesAsync(ct);

            // Se salvou, retorna mensagem de sucesso, caso contrário, mensagem de falha.
            return saved ? (true, "Modelo excluido com sucesso.") : (false, "Falha ao excluir modelo.");
        }

    }
}