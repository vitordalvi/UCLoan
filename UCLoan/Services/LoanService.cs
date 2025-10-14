using UCLoan.Repositories;
using UCLoan.Models;
using Microsoft.AspNetCore.Mvc;

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

        public async Task<(bool Success, string Error)> CreateAsync(string userId, 
            DateTime startDate,
            DateTime endDate,
            CancellationToken ct)
        {
            var user = _adminRepository.GetByIdAsync(userId);

            if (user.Result == null)
            {
                return (false, "Usuário não encontrado");
            }

            var loan = new Loan
            {
                User = user.Result,
                StartDate = startDate,
                EndDate = endDate,
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
    }
}
