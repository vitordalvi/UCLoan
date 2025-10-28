using System;
using System.ComponentModel.DataAnnotations;
using UCLoan.Constants;

namespace UCLoan.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class RoleAttribute : ValidationAttribute
    {
        // Valida se o Role é válido
        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            return Enum.TryParse(typeof(Role), value.ToString(), true, out var parsed) && parsed != null;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} inválido.";
        }
    }
}