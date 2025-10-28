using System;
using System.Collections.Generic;
using System.Linq;

namespace UCLoan.Utils
{
    public static class EnumHelper
    {
        // Retorna uma lista com todos os nomes de um enum.
        public static List<string> GetNames<T>() where T : Enum
        {
            return new List<string>(Enum.GetNames(typeof(T)));
        }

        // Retorna uma lista de pares (valor, nome) de um enum.
        public static List<(int Value, string Name)> GetValuesAndNames<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                       .Cast<T>()
                       .Select(e => (Convert.ToInt32(e), e.ToString()))
                       .ToList();
        }

        // Retorna o nome do enum correspondente ao valor informado.
        public static string GetName<T>(T value) where T : Enum
        {
            return Enum.GetName(typeof(T), value) ?? string.Empty;
        }

        // Tenta converter um número para o enum correspondente.
        // Retorna null se o valor não existir no enum.
        public static T? GetEnumByValue<T>(int value) where T : struct, Enum
        {
            return Enum.IsDefined(typeof(T), value) ? (T)Enum.ToObject(typeof(T), value) : null;
        }

        // Tenta converter um nome para o enum correspondente (case-insensitive).
        // Retorna null se o nome for inválido.
        public static T? GetEnumByName<T>(string name) where T : struct, Enum
        {
            if (Enum.TryParse<T>(name, true, out var result))
                return result;

            return null;
        }
    }
}
