using UCLoan.Constants;
using UCLoan.Models;

namespace UCLoan.Repositories
{
    public interface IEquipmentRepository
    {
        Task<List<Equipment>> GetAllEquipmentAsync(CancellationToken ct = default);
        Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<EquipmentModel?> GetModelOfEquipment(Equipment equipment, CancellationToken ct = default);
        Task AddAsync(Equipment equipment, CancellationToken ct = default);
        Task UpdateAsync(Equipment equipment, CancellationToken ct = default);
        Task DeleteAsync(Equipment equipment, CancellationToken ct = default);
        Task<List<EquipmentConstants.EquipmentPhysicalStatus>> GetAllEquipmentPhysicalStatus(CancellationToken ct = default);
        Task<List<EquipmentConstants.EquipmentLoanStatus>> GetAllEquipmentLoanStatus(CancellationToken ct = default);
        Task<List<EquipmentModel>> GetAllEquipmentModelsAsync(CancellationToken ct = default);
        Task<EquipmentModel?> GetModelByIdAsync(int id, CancellationToken ct = default);
        Task AddModelAsync(EquipmentModel model, CancellationToken ct = default);
        Task UpdateModelAsync(EquipmentModel model, CancellationToken ct = default);
        Task DeleteEquipmentModelAsync(EquipmentModel model, CancellationToken ct = default);
        Task<bool> SaveChangesAsync(CancellationToken ct = default);
    }
}
