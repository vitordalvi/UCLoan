using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using UCLoan.DTOs.Authentication;
using UCLoan.Entities;
using UCLoan.Repository;

namespace UCLoan.Services
{
    public class AuthService
    {
        private readonly IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        // Obter usuário por email
        public Task<User> GetUserByEmailAsync(string email) =>
            _authRepository.GetUserByEmailAsync(email);

        //public async Task<(bool Success, string Error)> RegisterAsync(UserDTO request)
        //{
        //    if (await _authRepository.GetUserByEmailAsync(request.Email) != null)
        //    {
        //        return (false, "O usuário já existe.");
        //    }


            
        //}
    }
}
