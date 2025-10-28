using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace UCLoan.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class CpfAttribute : ValidationAttribute
    {
        // Valida se um CPF é válido
        public override bool IsValid(object? value)
        {
            // Se o campo for nulo, vai ser válido. Usar [Required] para campos obrigatórios
            if (value == null)
                return true;

            string cpf = value.ToString()!.Replace(".", "").Replace("-", "").Trim();

            if (cpf.Length != 11 || cpf.Distinct().Count() == 1)
                return false;

            int[] multipliers1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multipliers2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int sum = 0;

            for (int i = 0; i < 9; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multipliers1[i];

            int remainder = sum % 11;
            int firstDigit = remainder < 2 ? 0 : 11 - remainder;

            tempCpf += firstDigit;
            sum = 0;

            for (int i = 0; i < 10; i++)
                sum += int.Parse(tempCpf[i].ToString()) * multipliers2[i];

            remainder = sum % 11;
            int secondDigit = remainder < 2 ? 0 : 11 - remainder;

            return cpf.EndsWith(firstDigit.ToString() + secondDigit.ToString());
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} inválido.";
        }
    }
}
