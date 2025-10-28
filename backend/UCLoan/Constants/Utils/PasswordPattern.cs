namespace UCLoan.Constants.Utils
{
    // Define os padrões de senha disponíveis
    public enum PasswordPattern
    {
        DefaultPattern,
        // 8 caracteres, uma letra e um número
        // (^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$)
        MinimumEight_LetterAndNumber,
        // 8 caracteres, uma letra, um número e um caractere especial
        // (^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$)
        MinimumEight_LetterNumberSpecial,
        // 8 caracteres, uma letra maiúscula, uma letra minúscula e um número
        // (^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$)
        MinimumEight_UpperLowerNumber,
        // 8 caracteres, uma letra maiúscula, uma letra minúscula, um número e um caractere especial
        // (^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$)
        MinimumEight_UpperLowerNumberSpecial,
        // 8 a 10 caracteres, uma letra maiúscula, uma letra minúscula, um número e um caractere especial
        // (^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,10}$)
        EightToTen_UpperLowerNumberSpecial
    }
}
