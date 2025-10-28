using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace UCLoan.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class NameAttribute : ValidationAttribute
    {
        // Regex para permitir letras (maiúsculas/minúsculas, acentos), espaços, apóstrofes e hífens
        private const string NameRegex = @"^[A-Za-zÀ-úa-üÁ-ÜçÇ\s'\-]{2,100}$";

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            var name = value.ToString()!.Trim();

            return Regex.IsMatch(name, NameRegex);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"O campo Nome deve conter de 2 a 100 caracteres e apenas letras, espaços, apóstrofes ou hífens.";
        }
    }
}
