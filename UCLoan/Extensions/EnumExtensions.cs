using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace UCLoan.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            var memberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();

            if (memberInfo != null)
            {
                var displayAttr = memberInfo.GetCustomAttribute<DisplayAttribute>();
                if (displayAttr != null)
                    return displayAttr.GetName()!;
            }

            return enumValue.ToString();
        }
    
         public static string GetDisplayNameViews(Enum enumValue)
            {
                return enumValue.GetType()
                    .GetMember(enumValue.ToString())
                    .First()
                    .GetCustomAttribute<DisplayAttribute>()?
                    .GetName() ?? enumValue.ToString();
            }
            public static Dictionary<int, string> GetDisplayNames<TEnum>() where TEnum : Enum
            {
                return Enum.GetValues(typeof(TEnum))
                    .Cast<TEnum>()
                    .ToDictionary(
                        e => Convert.ToInt32(e),
                        e => GetDisplayName(e)
                    );
            }
        }
    }
