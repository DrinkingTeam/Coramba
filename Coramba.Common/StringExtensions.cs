using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Coramba.Common
{
    public static class StringExtensions
    {
        public static string PascalToSnakeCase(this string str)
            => string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString())).ToLower();

        public static string PascalToKebabCase(this string str)
            => str == null ? null : Regex.Replace(str, "([a-z])([A-Z])", "$1-$2").ToLower();

        public static string[] SplitPascalCase(this string str)
            => str
                .Slice(char.IsUpper) // slice the array of chars to array of arrays of chars starting with upper char
                .Select(a => new string(a.ToArray()).ToLower()) // joining nested arrays of chars to string, and lowering it
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

        public static string ToCamelCase(this IEnumerable<string> strings)
            => strings
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s.ToLower())
                .Flatten("_");

        public static string RemoveUnderscores(this string str)
            => str.Replace("_", string.Empty);

        public static string TrimEnd(this string str, string suffix)
            => str != null && str.EndsWith(suffix) ? str.Substring(0, str.Length - suffix.Length) : str;

        public static string TrimStart(this string str, string prefix)
            => str != null && str.StartsWith(prefix) ? str.Substring(prefix.Length) : str;

        public static string Flatten<T>(this IEnumerable<T> strings, string separator = "")
        {
            if (strings == null) throw new ArgumentNullException(nameof(strings));
            return string.Join(separator, strings);
        }
    }
}
