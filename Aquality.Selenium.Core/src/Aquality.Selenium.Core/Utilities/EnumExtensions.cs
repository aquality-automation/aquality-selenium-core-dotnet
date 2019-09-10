using System;
using System.Diagnostics.Contracts;

namespace Aquality.Selenium.Core.Utilities
{
    /// <summary>
    /// Simplifies converting values to enum
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Converts object to enum.
        /// If the object is int - casts it to enum directly;
        /// Otherwise, calls <see cref="object.ToString()"/> and then <see cref="ToEnum{T}(string)"/>
        /// </summary>
        /// <typeparam name="T">Target enum type</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted enum value</returns>
        public static T ToEnum<T>(this object value) where T : struct, IConvertible
        {
            return value is int
                ? (T)(object)(int)(long)value
                : value.ToString().ToEnum<T>();
        }

        /// <summary>
        /// Converts object to enum.
        /// Asserts via <see cref="Contract.Assert(bool)"/> that <see cref="Type.IsEnum"/>.
        /// Then calls <see cref="Enum.Parse(Type, string)"/> against passed values
        /// </summary>
        /// <typeparam name="T">Target enum type</typeparam>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted enum value</returns>
        public static T ToEnum<T>(this string value) where T : struct, IConvertible
        {
            var type = typeof(T);
            Contract.Assert(type.IsEnum, "T must be an enum type");
            return (T)Enum.Parse(type, value, ignoreCase: true);
        }
    }
}
