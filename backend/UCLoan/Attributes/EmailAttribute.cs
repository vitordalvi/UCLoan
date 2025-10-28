using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace UCLoan.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class EmailAttribute : ValidationAttribute
    {
        // Valida se um e-mail é válido
        public override bool IsValid(object? value)
        {
            // Se o campo for nulo, vai ser válido. Usar [Required] para campos obrigatórios
            if (value == null)
                return true;

            try
            {
                var _ = new MailAddress(value.ToString()!);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} inválido.";
        }
    }
}
