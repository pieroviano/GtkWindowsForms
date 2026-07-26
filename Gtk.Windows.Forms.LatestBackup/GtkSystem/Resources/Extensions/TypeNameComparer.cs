using System.Reflection;
using System.Runtime.CompilerServices;

namespace GtkSystem.Resources.Extensions;

internal sealed class TypeNameComparer : IEqualityComparer<string>
{
    private static readonly char[] s_whiteSpaceChars = new char[4] { ' ', '\n', '\r', '\t' };

    public static TypeNameComparer Instance { get; } = new();

    public bool Equals(string assemblyQualifiedTypeName1, string assemblyQualifiedTypeName2)
    {
        if (assemblyQualifiedTypeName1 == null)
        {
            throw new ArgumentNullException(nameof(assemblyQualifiedTypeName1));
        }
        if (assemblyQualifiedTypeName2 == null)
        {
            throw new ArgumentNullException(nameof(assemblyQualifiedTypeName2));
        }
        if ((object)assemblyQualifiedTypeName1 == assemblyQualifiedTypeName2)
        {
            return true;
        }
        var typeNameSpan1 = assemblyQualifiedTypeName1.AsSpan().TrimStart(s_whiteSpaceChars);
        var typeNameSpan2 = assemblyQualifiedTypeName2.AsSpan().TrimStart(s_whiteSpaceChars);
        var mainTypeName1 = ReadTypeName(typeNameSpan1);
        var mainTypeName2 = ReadTypeName(typeNameSpan2);
        if (!MemoryExtensions.Equals(mainTypeName1, mainTypeName2, StringComparison.Ordinal))
        {
            return false;
        }
        typeNameSpan1 = ((typeNameSpan1.Length > mainTypeName1.Length) ? typeNameSpan1.Slice(mainTypeName1.Length + 1).TrimStart(s_whiteSpaceChars) : ReadOnlySpan<char>.Empty);
        typeNameSpan2 = ((typeNameSpan2.Length > mainTypeName2.Length) ? typeNameSpan2.Slice(mainTypeName2.Length + 1).TrimStart(s_whiteSpaceChars) : ReadOnlySpan<char>.Empty);
        var assemblyName1 = ReadAssemblySimpleName(typeNameSpan1);
        var assemblyName2 = ReadAssemblySimpleName(typeNameSpan2);
        if ((assemblyName1.IsEmpty && !typeNameSpan1.IsEmpty) || (assemblyName2.IsEmpty && !typeNameSpan2.IsEmpty))
        {
            return false;
        }
        if (assemblyName1.IsEmpty)
        {
            if (!assemblyName2.IsEmpty)
            {
                return IsMscorlib(assemblyName2);
            }
            return true;
        }
        if (assemblyName2.IsEmpty)
        {
            return IsMscorlib(assemblyName1);
        }
        if (!MemoryExtensions.Equals(assemblyName1, assemblyName2, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        if (IsMscorlib(assemblyName1))
        {
            return true;
        }
        var assemblyObj1 = new AssemblyName(typeNameSpan1.ToString());
        var assemblyObj2 = new AssemblyName(typeNameSpan2.ToString());
        if (assemblyObj1.CultureInfo?.LCID != assemblyObj2.CultureInfo?.LCID)
        {
            return false;
        }
        var publicKeyToken1 = assemblyObj1.GetPublicKeyToken();
        var publicKeyToken2 = assemblyObj2.GetPublicKeyToken();

        return publicKeyToken1.AsSpan().SequenceEqual(publicKeyToken2);
    }

    public int GetHashCode(string assemblyQualifiedTypeName)
    {
        var trimmedTypeName = assemblyQualifiedTypeName.AsSpan().TrimStart(s_whiteSpaceChars);
        var mainTypeName = ReadTypeName(trimmedTypeName);
        var hash = 0;
        for (var i = 0; i < mainTypeName.Length; i++)
        {
            var prevHash = hash;
            var ch = mainTypeName[i];
            hash = GtkSystem.Numerics.Hashing.HashHelpers.Combine(prevHash, ch.GetHashCode());
        }

        return hash;
    }

    private static bool IsMscorlib(ReadOnlySpan<char> assemblyName)
    {
        return MemoryExtensions.Equals(assemblyName, "mscorlib".AsSpan(), StringComparison.OrdinalIgnoreCase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<char> ReadAssemblySimpleName(ReadOnlySpan<char> assemblyName)
    {
        var num = assemblyName.IndexOf(',');
        if (num != -1)
        {
            return assemblyName.Slice(0, num).TrimEnd(s_whiteSpaceChars);
        }

        return assemblyName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<char> ReadTypeName(ReadOnlySpan<char> assemblyQualifiedTypeName)
    {
        var num = assemblyQualifiedTypeName.IndexOf(',');
        if (num != -1)
        {
            return assemblyQualifiedTypeName.Slice(0, num);
        }

        return assemblyQualifiedTypeName;
    }
}