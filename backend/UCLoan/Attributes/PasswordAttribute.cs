using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using UCLoan.Constants.Utils;

namespace UCLoan.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasswordAttribute : ValidationAttribute
    {
        private readonly PasswordPattern _pattern;

        public PasswordAttribute(PasswordPattern pattern)
        {
            _pattern = pattern;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            var password = value.ToString();

            // Define o regex com base no padrão selecionado
            string regex = _pattern switch
            {
                // Padrão, sem restrições específicas
                PasswordPattern.DefaultPattern => @".+",
                // 8 caracteres, uma letra e um número
                PasswordPattern.MinimumEight_LetterAndNumber => @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
                // 8 caracteres, uma letra, um número e um caractere especial
                PasswordPattern.MinimumEight_LetterNumberSpecial => @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$",
                // 8 caracteres, uma letra maiúscula, uma letra minúscula e um número
                PasswordPattern.MinimumEight_UpperLowerNumber => @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$",
                // 8 caracteres, uma letra maiúscula, uma letra minúscula, um número e um caractere especial
                PasswordPattern.MinimumEight_UpperLowerNumberSpecial => @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                // 8 a 10 caracteres, uma letra maiúscula, uma letra minúscula, um número e um caractere especial
                PasswordPattern.EightToTen_UpperLowerNumberSpecial => @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,10}$",
                _ => throw new ArgumentOutOfRangeException()
            };

            return Regex.IsMatch(password!, regex);
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage ?? $"{name} não atende ao padrão de senha definido.";
        }
    }
}
