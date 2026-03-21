using System;
using System.Runtime.CompilerServices;
using GtkSystem.Resources;

namespace System;

internal static class SystemResources
{
    private static ResourceManager s_resourceManager;

    internal static ResourceManager ResourceManager => s_resourceManager ?? (s_resourceManager = new GtkSystem.Resources.ResourceManager(typeof(System.Properties.Resources)));

    internal static string Format(string resourceFormat, object p1)
    {
        if (UsingResourceKeys())
        {
            return string.Join(", ", resourceFormat, p1);
        }

        return string.Format(resourceFormat, p1);
    }

    internal static string Format(string resourceFormat, object p1, object p2)
    {
        if (UsingResourceKeys())
        {
            return string.Join(", ", resourceFormat, p1, p2);
        }

        return string.Format(resourceFormat, p1, p2);
    }

    internal static string Format(string resourceFormat, object p1, object p2, object p3)
    {
        if (UsingResourceKeys())
        {
            return string.Join(", ", resourceFormat, p1, p2, p3);
        }

        return string.Format(resourceFormat, p1, p2, p3);
    }

    internal static string Format(string resourceFormat, params object[] args)
    {
        if (args != null)
        {
            if (UsingResourceKeys())
            {
                return resourceFormat + ", " + string.Join(", ", args);
            }
            return string.Format(resourceFormat, args);
        }

        return resourceFormat;
    }

    internal static string Format(IFormatProvider provider, string resourceFormat, object p1)
    {
        if (UsingResourceKeys())
        {
            return string.Join(", ", resourceFormat, p1);
        }

        return string.Format(provider, resourceFormat, p1);
    }

    internal static string Format(IFormatProvider provider, string resourceFormat, object p1, object p2)
    {
        if (UsingResourceKeys())
        {
            return string.Join(", ", resourceFormat, p1, p2);
        }

        return string.Format(provider, resourceFormat, p1, p2);
    }

    internal static string Format(IFormatProvider provider, string resourceFormat, object p1, object p2, object p3)
    {
        if (UsingResourceKeys())
        {
            return string.Join(", ", resourceFormat, p1, p2, p3);
        }

        return string.Format(provider, resourceFormat, p1, p2, p3);
    }

    internal static string Format(IFormatProvider provider, string resourceFormat, params object[] args)
    {
        if (args != null)
        {
            if (UsingResourceKeys())
            {
                return resourceFormat + ", " + string.Join(", ", args);
            }
            return string.Format(provider, resourceFormat, args);
        }

        return resourceFormat;
    }

    internal static string GetResourceString(string resourceKey, string defaultString = null)
    {
        if (UsingResourceKeys())
        {
            return defaultString ?? resourceKey;
        }
        string text = null;
        try
        {
            text = ResourceManager.GetString(resourceKey);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.Write($"Unexpected exception in GetResourceString: {ex}");
        }
        if (defaultString != null && resourceKey.Equals(text))
        {
            return defaultString;
        }

        return text;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static bool UsingResourceKeys()
    {
        return false;
    }
}