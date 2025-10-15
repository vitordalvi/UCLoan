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
        private readonly IEquipmentRepository _equipmentRepository;
        private readonly IAdminRepository _adminRepository;
        public LoanService(ILoanRepository loanRepository, IEquipmentRepository equipmentRepository, IAdminRepository adminRepository)
        {
            _loanRepository = loanRepository;
            _equipmentRepository = equipmentRepository;
            _adminRepository = adminRepository;
        }

        public Task<List<Models.Loan>> GetAllLoansAsync(CancellationToken ct = default) =>
            _loanRepository.GetAllLoansAsync(ct);

        public Task<Models.Loan?> GetByIdAsync(int id, CancellationToken ct = default) => 
            _loanRepository.GetByIdAsync(id, ct);

        public async Task<(bool Success, string Error)> CreateAsync(string userEmail,
            int equipmentId,
            DateTime startDate,
            DateTime endDate,
            string description,
            CancellationToken ct = default)
        {
            var user =  await _adminRepository.GetByEmailAsync(userEmail);
            var equipment = await _equipmentRepository.GetByIdAsync(equipmentId, ct);


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

        public async Task<(bool Success, string Error)> UpdateAsync(int id,
            DateTime startDate,
            DateTime endDate,
            CancellationToken ct = default)
        {
            var loan = await _loanRepository.GetByIdAsync(id, ct);

            if (loan == null)
            {
                return (false, "Empréstimo não encontrado.");
            }

            loan.StartDate = startDate;
            loan.EndDate = endDate;

            await _loanRepository.UpdateAsync(loan, ct);
            var saved = await _loanRepository.SaveChangesAsync(ct);

            return saved ? (true, "Empréstimo atualizado com sucesso.") : (false, "Erro ao atualizar o empréstimo.");
        }

        public async Task<(bool Success, string Error)> DeleteAsync(int id, CancellationToken ct = default)
        {
            var loan = await _loanRepository.GetByIdAsync(id, ct);
            if (loan == null)
            {
                return (false, "Empréstimo não encontrado.");
            }
            await _loanRepository.DeleteAsync(loan, ct);
            var saved = await _loanRepository.SaveChangesAsync(ct);
            return saved ? (true, "Empréstimo deletado com sucesso.") : (false, "Erro ao deletar o empréstimo.");
        }

        public async Task<List<SelectListItem>> GetAllEquipmentsAvailableSelectListAsync()
        {
            var equipments = await _equipmentRepository.GetAllEquipmentAsync();

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

        public async Task<List<Equipment>> GetAllActiveLoans()
        {
            var equipments = await _equipmentRepository.GetAllEquipmentAsync();
            var statusList = new[]
            {
                EquipmentConstants.EquipmentLoanStatus.Borrowed,
                EquipmentConstants.EquipmentLoanStatus.Overdue,
                EquipmentConstants.EquipmentLoanStatus.Returned
            };

            var loanedEquipments = equipments
                .Where(e => statusList.Contains(e.LoanStatus))
                .ToList();

            return loanedEquipments;
        }

        public async Task<List<Equipment>> GetAllOverdueLoans()
        {
            var equipments = await _equipmentRepository.GetAllEquipmentAsync();

            var overdueLoans = equipments
                .Where(e => e.LoanStatus == EquipmentConstants.EquipmentLoanStatus.Overdue)
                .ToList();

            return overdueLoans;
        }
    }
}
