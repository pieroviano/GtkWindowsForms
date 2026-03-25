using System.Resources;

namespace GtkSystem.Resources.Extensions;

internal static class ResXExtensions
{
    public static string OrThrowIfNull(this string value)
    {
        if (value == null)
        {
            throw new Exception("string Is Null Or Empty");
        }
        else
            return value;
    }

    public static ResXFileRef OrThrowIfNull(this ResXFileRef value)
    {
        if (value == null)
        {
            throw new Exception("string is null");
        }
        else
            return value;
    }

    public static string OrThrowIfNullOrEmpty(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new Exception("string Is Null Or Empty");
        }
        else
            return value;
    }
}