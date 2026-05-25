using System.Text.RegularExpressions;

namespace OrderHub.Shared.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="string"/> manipulation.
    /// </summary>
    public static partial class StringExtensions
    {
        /// <summary>
        /// Converts a PascalCase or camelCase string to snake_case.
        /// For example, <c>"OrderNumber"</c> becomes <c>"order_number"</c>.
        /// </summary>
        /// <param name="self">The string to convert.</param>
        /// <returns>A lowercase snake_case representation of the input string.</returns>
        public static string ToSnakeCase(this string self)
            => StringExtensions.UpperToSnakeCaseRegex().Replace(self, "$1_$2").ToLowerInvariant();

        /// <summary>
        /// Source-generated regex that matches a word character immediately followed by an
        /// uppercase letter, used to insert underscore separators between words.
        /// </summary>
        [GeneratedRegex(@"(\w)([A-Z])")]
        private static partial Regex UpperToSnakeCaseRegex();
    }
}