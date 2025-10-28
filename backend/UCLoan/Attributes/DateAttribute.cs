using System;
using System.ComponentModel.DataAnnotations;

namespace UCLoan.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DateRangeFromTodayAttribute : ValidationAttribute
    {
        public int? MinDays { get; }
        public int? MaxDays { get; }

        // Use -1 para indicar que não quer limite
        public DateRangeFromTodayAttribute(int minDays = -1, int maxDays = -1)
        {
            if (minDays != -1) MinDays = minDays;
            if (maxDays != -1) MaxDays = maxDays;
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
                return true;

            if (value is not DateTime dateValue)
                return false;

            var today = DateTime.UtcNow.Date;

            if (MinDays.HasValue)
            {
                var minDate = today.AddDays(MinDays.Value);
                if (dateValue < minDate)
                    return false;
            }

            if (MaxDays.HasValue)
            {
                var maxDate = today.AddDays(MaxDays.Value);
                if (dateValue > maxDate)
                    return false;
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            if (MinDays.HasValue && MaxDays.HasValue)
                return $"{name} deve estar entre {MinDays} e {MaxDays} dias a partir de hoje.";

            if (MinDays.HasValue)
                return $"{name} deve ser pelo menos {MinDays} dias após hoje.";

            if (MaxDays.HasValue)
                return $"{name} deve ser no máximo em {MaxDays} dias a partir de hoje.";

            return $"{name} possui uma data inválida.";
        }
    }
}
