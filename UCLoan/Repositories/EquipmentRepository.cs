using Microsoft.EntityFrameworkCore;
using UCLoan.Constants;
using UCLoan.Data;
using UCLoan.Models;

namespace UCLoan.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EquipmentRepository(ApplicationDbContext context)
        {
            _context = context;   
        }

        // Equipamentos

        public Task<List<Equipment>> GetAllEquipmentAsync(CancellationToken ct = default) =>
            _context.Set<Equipment>()
            .Include(e => e.EquipmentModel)
            .ToListAsync(ct);

        public Task<Equipment?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _context.Set<Equipment>()
            .Include(e => e.EquipmentModel)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        public Task<EquipmentModel?> GetModelOfEquipment(Equipment equipment, CancellationToken ct = default) =>
            _context.Set<EquipmentModel>()
            .FirstOrDefaultAsync(m => m.Id == equipment.EquipmentModelId, ct);

        public async Task AddAsync(Equipment equipment, CancellationToken ct = default)
        {
            await _context.Set<Equipment>().AddAsync(equipment, ct);
        }

        public Task UpdateAsync(Equipment equipment, CancellationToken ct = default)
        {
            _context.Set<Equipment>().Update(equipment);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Equipment equipment, CancellationToken ct = default)
        {
            _context.Set<Equipment>().Remove(equipment);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
            _context.Set<Equipment>().AnyAsync(e => e.Id == id, ct);
        public Task<List<EquipmentConstants.EquipmentPhysicalStatus>> GetAllEquipmentPhysicalStatus(CancellationToken ct = default)
        {
            var values = Enum.GetValues(typeof(EquipmentConstants.EquipmentPhysicalStatus))
                .Cast<EquipmentConstants.EquipmentPhysicalStatus>()
                .ToList();

            return Task.FromResult(values);
        }

        public Task<List<EquipmentConstants.EquipmentLoanStatus>> GetAllEquipmentLoanStatus(CancellationToken ct = default)
        {
            var values = Enum.GetValues(typeof(EquipmentConstants.EquipmentLoanStatus))
                .Cast<EquipmentConstants.EquipmentLoanStatus>()
                .ToList();

            return Task.FromResult(values);
        }

        // EquipmentModel

        public Task<List<EquipmentModel>> GetAllEquipmentModelsAsync(CancellationToken ct = default) =>
            _context.Set<EquipmentModel>()
            .Include(m => m.Equipments)
            .OrderBy(m => m.Manufacturer)
            .ThenBy(m => m.Name)
            .ToListAsync(ct);

        public Task<EquipmentModel?> GetModelByIdAsync(int id, CancellationToken ct = default) =>
            _context.Set<EquipmentModel>()
                    .Include(m => m.Equipments)
                    .FirstOrDefaultAsync(m => m.Id == id, ct);

        public async Task AddModelAsync(EquipmentModel model, CancellationToken ct = default)
        {
            await _context.Set<EquipmentModel>().AddAsync(model, ct);
        }

        public async Task UpdateModelAsync(EquipmentModel model, CancellationToken ct = default)
        {
            _context.Set<EquipmentModel>().Update(model);
        }

        public Task DeleteEquipmentModelAsync(EquipmentModel equipmentModel, CancellationToken ct)
        {
            _context.Set<EquipmentModel>().Remove(equipmentModel);
            return Task.CompletedTask;
        }

        public async Task<bool> SaveChangesAsync(CancellationToken ct = default) =>
            (await _context.SaveChangesAsync(ct)) > 0;
    }
}
