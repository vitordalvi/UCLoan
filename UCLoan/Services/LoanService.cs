using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using UCLoan.Constants;
using UCLoan.Extensions;
using UCLoan.Models;
using UCLoan.Repositories;

namespace UCLoan.Services
{
    public class LoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly EquipmentService _equipmentService;
        private readonly AdminService _adminService;
        public LoanService(ILoanRepository loanRepository, EquipmentService equipmentService, AdminService adminService)
        {
            _loanRepository = loanRepository;
            _equipmentService = equipmentService;
            _adminService = adminService;
        }

        public Task<List<Loan>> GetAllLoansAsync(CancellationToken ct = default) =>
            _loanRepository.GetAllLoansAsync(ct);

        public Task<List<Loan>> GetUserLoansByEmailAsync(string userEmail, CancellationToken ct = default) =>
            _loanRepository.GetUserLoansByEmailAsync(userEmail, ct);

        public async Task<List<Equipment>> GetUserLoansEquipmentByEmailAsync(string userEmail, CancellationToken ct = default)
        {
            var equipments = await _loanRepository.GetUserLoansEquipmentByEmailAsync(userEmail, ct);
            return equipments.Where(e => e != null).ToList()!;
        }

        public Task<Models.Loan?> GetByIdAsync(int id, CancellationToken ct = default) => 
            _loanRepository.GetByIdAsync(id, ct);

        public async Task<(bool Success, string Error)> CreateAsync(string userEmail,
            int equipmentId,
            DateTime startDate,
            DateTime endDate,
            string description,
            CancellationToken ct = default)
        {
            var user =  await _adminService.GetByEmailAsync(userEmail);
            var equipment = await _equipmentService.GetByIdAsync(equipmentId, ct);


            if (user == null)
            {
                return (false, "Usuário não encontrado");
            }

            if (equipment == null)
            {
                return (false, "O equipamento é inválido.");
            }

            equipment.LoanStatus = EquipmentConstants.EquipmentLoanStatus.Borrowed;

            var loan = new Loan
            {
                User = user,
                Equipment = equipment,
                StartDate = startDate,
                EndDate = endDate,
                Description = description,
            };

            await _loanRepository.AddAsync(loan, ct);
            var saved = await _loanRepository.SaveChangesAsync();

            return saved ? (true, "Empréstimo feito com sucesso.") : (false, "Erro ao fazer o empréstimo.");
        }

        public async Task<(bool Success, string Error)> UpdateAsync(
            int id,
            int equipmentId,
            EquipmentConstants.EquipmentLoanStatus loanStatus,
            EquipmentConstants.EquipmentPhysicalStatus physicalStatus,
            DateTime startDate,
            DateTime endDate,
            string description,
            CancellationToken ct = default)
        {
            var loan = await _loanRepository.GetByIdAsync(id, ct);
            var equipment = await _equipmentService.GetByEquipmentIdAsync(equipmentId, ct);

            if (loan == null)
            {
                return (false, "Empréstimo não encontrado.");
            }

            if (equipment == null)
            {
                return (false, "O equipamento é inválido.");
            }

            ////var statusResult = await _equipmentService.ChangeLoanStatusAsync(equipmentId, loanStatus, ct);

            //if (!statusResult.Success)
            //{
            //    return (false, statusResult.Error ?? "Erro ao atualizar o status do equipamento.");
            //}

            loan.Equipment = equipment;
            loan.Equipment.LoanStatus = loanStatus;
            loan.Equipment.PhysicalStatus = physicalStatus;
            loan.StartDate = startDate;
            loan.EndDate = endDate;
            loan.Description = description;

            await _loanRepository.UpdateAsync(loan, ct);
            var saved = await _loanRepository.SaveChangesAsync(ct);

            return saved ? (true, "Empréstimo atualizado com sucesso.") : (false, "Erro ao atualizar o empréstimo.");
        }

        public async Task<(bool Success, string? Error)> ConfirmReceiptAsync(int id, CancellationToken ct = default)
        {
            var loan = await _loanRepository.GetByIdAsync(id, ct);

            if (loan == null)
            {
                return (false, "Empréstimo não encontrado.");
            }
            var equipment = loan.Equipment;

            if (equipment == null)
            {
                return (false, "Equipamento associado ao empréstimo não encontrado.");
            }

            loan.Equipment = null!;
            equipment.LoanStatus = EquipmentConstants.EquipmentLoanStatus.Returned;
            loan.EndDate = DateTime.UtcNow;

            await _loanRepository.UpdateAsync(loan, ct);
            var saved = await _loanRepository.SaveChangesAsync(ct);

            return saved ? (true, null) : (false, "Erro ao confirmar o recebimento do equipamento.");
        }

        public async Task<(bool Success, string Error)> DeleteAsync(int id, CancellationToken ct = default)
        {
            var blockedStatus = new[]
            {
                EquipmentConstants.EquipmentLoanStatus.Borrowed,
                EquipmentConstants.EquipmentLoanStatus.Overdue,
                EquipmentConstants.EquipmentLoanStatus.Unavailable
            };

            var loan = await _loanRepository.GetByIdAsync(id, ct);
            var loanStatus = await _loanRepository.GetLoanStatusAsync(id, ct);

            if (loan == null)
            {
                return (false, "Empréstimo não encontrado.");
            }

            if (blockedStatus.Contains(loanStatus))
            {
                return (false, "Não é possível deletar um empréstimo com status Emprestado, Atrasado ou Indisponível.");
            }

            
            await _loanRepository.DeleteAsync(loan, ct);
            var saved = await _loanRepository.SaveChangesAsync(ct);
            return saved ? (true, "Empréstimo deletado com sucesso.") : (false, "Erro ao deletar o empréstimo.");
        }

        public async Task<List<SelectListItem>> GetAllEquipmentsAvailableSelectListAsync()
        {
            var equipments = await _equipmentService.GetAllEquipmentAsync();

            var availableEquipments = equipments
                .Where(e => e.LoanStatus == EquipmentConstants.EquipmentLoanStatus.Available)
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = $"Patrimônio: {e.EquipmentId} - {e.EquipmentModel.Manufacturer} {e.EquipmentModel?.Name}"
                })
                .ToList();

            return availableEquipments;
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

        public async Task<List<Equipment>> GetAllActiveLoans()
        {
            var equipments = await _equipmentService.GetAllEquipmentAsync();
            var statusList = new[]
            {
                EquipmentConstants.EquipmentLoanStatus.Borrowed,
                EquipmentConstants.EquipmentLoanStatus.Overdue,
            };

            var loanedEquipments = equipments
                .Where(e => statusList.Contains(e.LoanStatus))
                .ToList();

            return loanedEquipments;
        }

        public async Task<List<Equipment>> GetAllOverdueLoans()
        {
            var equipments = await _equipmentService.GetAllEquipmentAsync();

            var overdueLoans = equipments
                .Where(e => e.LoanStatus == EquipmentConstants.EquipmentLoanStatus.Overdue)
                .ToList();

            return overdueLoans;
        }

        public async Task<List<Equipment>> GetAllLoanStatusAsync()
        {
            var equipments = await _equipmentService.GetAllEquipmentAsync();
            var statusList = new[]
            {
                EquipmentConstants.EquipmentLoanStatus.Borrowed,
                EquipmentConstants.EquipmentLoanStatus.Overdue,
                EquipmentConstants.EquipmentLoanStatus.Returned,
                EquipmentConstants.EquipmentLoanStatus.Available,
                EquipmentConstants.EquipmentLoanStatus.Unavailable
            };

            var loanedEquipmentsStatus = equipments
                .Where(e => statusList.Contains(e.LoanStatus))
                .ToList();

            return loanedEquipmentsStatus;
        }

        
    }
}
