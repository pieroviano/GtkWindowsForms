using System.Reflection;
using System.Security;

namespace System.Windows.Forms;

internal static class SecurityUtils
{

    internal static object? SecureCreateInstance(Type? type)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        // When caller used the simple overload, allow creating instances of non-visible types
        // by trying non-public constructors. If the caller explicitly passes allowNonPublic=false
        // (via the other overload) we honor that decision.
        var allowNonPublicForDefault = !type.IsVisible;
        return SecureCreateInstance(type, null, allowNonPublicForDefault);
    }

    internal static object? SecureCreateInstance(Type? type, object[]? args, bool allowNonPublic)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        var bindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance;

        // If caller requested non-public creation but reflection permission is absent, respect that.
        if (allowNonPublic && !HasReflectionPermission(type))
        {
            allowNonPublic = false;
        }

        if (allowNonPublic)
        {
            bindingFlag |= BindingFlags.NonPublic;
        }

        return Activator.CreateInstance(type, bindingFlag, null, args, null);
    }

    private static bool HasReflectionPermission(Type? _)
    {
        bool flag;
        try
        {
            flag = true;
        }
        catch (SecurityException)
        {
            return false;
        }

        return flag;
    }
}