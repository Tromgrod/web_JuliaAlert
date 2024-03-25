using System;
using System.ComponentModel.DataAnnotations;

namespace LIB.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute[] displayAttribute && displayAttribute.Length > 0
                ? displayAttribute[0].Name
                : enumValue.ToString();
        }
    }
}