#nullable enable
using System.Text.RegularExpressions;

namespace Prepare.Common.Text;

public static class StringExtensionsAllFrameworks
{
    public static string? ReplaceIgnoreCase(this string? input, string? oldValue, string? newValue)
    {
        return string.IsNullOrEmpty(input) || 
               string.IsNullOrEmpty(oldValue) ? input : 
            Regex.Replace(input, Regex.Escape(oldValue), newValue ?? string.Empty, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
    }
}