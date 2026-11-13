using System.Resources;

namespace Gtk.Resources.Extensions;

internal static class ResXExtensions
{
    public static string OrThrowIfNull(this string? value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("string Is Null Or Empty");
        }
        else
            return value;
    }

    public static ResXFileRef OrThrowIfNull(this ResXFileRef? value)
    {
        if (value == null)
        {
            throw new ArgumentNullException("ResXFileRef is null");
        }
        else
            return value;
    }

    public static string? OrThrowIfNullOrEmpty(this string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("string Is Null Or Empty");
        }
        else
            return value;
    }
}