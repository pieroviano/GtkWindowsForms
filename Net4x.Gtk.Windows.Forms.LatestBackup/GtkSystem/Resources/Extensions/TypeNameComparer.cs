using System.Reflection;
using System.Runtime.CompilerServices;

namespace GtkSystem.Resources.Extensions;

internal sealed class TypeNameComparer : IEqualityComparer<string>
{
    private static readonly char[] s_whiteSpaceChars = new char[4] { ' ', '\n', '\r', '\t' };

    public static TypeNameComparer Instance { get; } = new TypeNameComparer();

    public bool Equals(string assemblyQualifiedTypeName1, string assemblyQualifiedTypeName2)
    {
        if (assemblyQualifiedTypeName1 == null)
        {
            throw new ArgumentNullException("assemblyQualifiedTypeName1");
        }
        if (assemblyQualifiedTypeName2 == null)
        {
            throw new ArgumentNullException("assemblyQualifiedTypeName2");
        }
        if ((object)assemblyQualifiedTypeName1 == assemblyQualifiedTypeName2)
        {
            return true;
        }
        var typeName1Span = assemblyQualifiedTypeName1.AsSpan().TrimStart(s_whiteSpaceChars);
        var typeName2Span = assemblyQualifiedTypeName2.AsSpan().TrimStart(s_whiteSpaceChars);
        var firstTypeName = ReadTypeName(typeName1Span);
        var secondTypeName = ReadTypeName(typeName2Span);
        if (!MemoryExtensions.Equals(firstTypeName, secondTypeName, StringComparison.Ordinal))
        {
            return false;
        }
        typeName1Span = typeName1Span.Length > firstTypeName.Length ? typeName1Span.Slice(firstTypeName.Length + 1).TrimStart(s_whiteSpaceChars) : ReadOnlySpan<char>.Empty;
        typeName2Span = typeName2Span.Length > secondTypeName.Length ? typeName2Span.Slice(secondTypeName.Length + 1).TrimStart(s_whiteSpaceChars) : ReadOnlySpan<char>.Empty;
        var firstAssemblyName = ReadAssemblySimpleName(typeName1Span);
        var secondAssemblyName = ReadAssemblySimpleName(typeName2Span);
        if (firstAssemblyName.IsEmpty && !typeName1Span.IsEmpty || secondAssemblyName.IsEmpty && !typeName2Span.IsEmpty)
        {
            return false;
        }
        if (firstAssemblyName.IsEmpty)
        {
            if (!secondAssemblyName.IsEmpty)
            {
                return IsMscorlib(secondAssemblyName);
            }
            return true;
        }
        if (secondAssemblyName.IsEmpty)
        {
            return IsMscorlib(firstAssemblyName);
        }
        if (!MemoryExtensions.Equals(firstAssemblyName, secondAssemblyName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
        if (IsMscorlib(firstAssemblyName))
        {
            return true;
        }
        var assemblyName = new AssemblyName(typeName1Span.ToString());
        var assemblyName2Full = new AssemblyName(typeName2Span.ToString());
        if (assemblyName.CultureInfo?.LCID != assemblyName2Full.CultureInfo?.LCID)
        {
            return false;
        }
        var publicKeyToken = assemblyName.GetPublicKeyToken();
        var otherPublicKeyToken = assemblyName2Full.GetPublicKeyToken();

        return publicKeyToken.AsSpan().SequenceEqual(otherPublicKeyToken);
    }

    public int GetHashCode(string assemblyQualifiedTypeName)
    {
        var typeNameSpan = assemblyQualifiedTypeName.AsSpan().TrimStart(s_whiteSpaceChars);
        var typeName = ReadTypeName(typeNameSpan);
        var hashCode = 0;
        for (var i = 0; i < typeName.Length; i++)
        {
            var previousHash = hashCode;
            var currentChar = typeName[i];
            hashCode = GtkSystem.Numerics.Hashing.HashHelpers.Combine(previousHash, currentChar.GetHashCode());
        }

        return hashCode;
    }

    private static bool IsMscorlib(ReadOnlySpan<char> assemblyName)
    {
        return MemoryExtensions.Equals(assemblyName, "mscorlib".AsSpan(), StringComparison.OrdinalIgnoreCase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<char> ReadAssemblySimpleName(ReadOnlySpan<char> assemblyName)
    {
        var commaIndex = assemblyName.IndexOf(',');
        if (commaIndex != -1)
        {
            return assemblyName.Slice(0, commaIndex).TrimEnd(s_whiteSpaceChars);
        }

        return assemblyName;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ReadOnlySpan<char> ReadTypeName(ReadOnlySpan<char> assemblyQualifiedTypeName)
    {
        var commaIndex = assemblyQualifiedTypeName.IndexOf(',');
        if (commaIndex != -1)
        {
            return assemblyQualifiedTypeName.Slice(0, commaIndex);
        }

        return assemblyQualifiedTypeName;
    }
}