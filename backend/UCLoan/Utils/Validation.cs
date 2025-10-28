using UCLoan.Attributes;

namespace UCLoan.Utils
{
    public static class Validation
    {
        // Retorna true ou false, para validar CPF
        public static bool isValidCPF(string cpf) =>
            new CpfAttribute().IsValid(cpf);

        // Retorna true ou false, para validar e-mail
        public static bool isValidEmail(string email) =>
            new EmailAttribute().IsValid(email);

        // Retorna true ou false, para validar nome
        public static bool isValidName(string name) =>
            new NameAttribute().IsValid(name);

        // Retorna true ou false, para validar função/role
        public static bool isValidRole(string role, IEnumerable<string> validRoles) =>
            validRoles.Contains(role);

        // Retorna true ou false, para validar se a data é válida
        public static bool isValidDate(DateTime date, int minDays, int maxDays) =>
            new DateRangeFromTodayAttribute(minDays, maxDays).IsValid(date);
    }
}
