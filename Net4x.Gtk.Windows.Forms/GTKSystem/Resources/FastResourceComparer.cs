using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace GtkSystem.Resources;

internal sealed class FastResourceComparer : IComparer, IEqualityComparer, IComparer<string>, IEqualityComparer<string>
{
    internal static readonly FastResourceComparer Default = new FastResourceComparer();

    public int Compare(object a, object b)
    {
        if (a == b)
        {
            return 0;
        }
        var strA = (string)a;
        var strB = (string)b;

        return string.CompareOrdinal(strA, strB);
    }

    public int Compare(string a, string b)
    {
        return string.CompareOrdinal(a, b);
    }

    public unsafe static int CompareOrdinal(string a, byte[] bytes, int bCharLength)
    {
        var stringIndex = 0;
        var comparisonResult = 0;
        var compareLength = a.Length;
        if (compareLength > bCharLength)
        {
            compareLength = bCharLength;
        }
        if (bCharLength == 0)
        {
            if (a.Length != 0)
            {
                return -1;
            }
            return 0;
        }
        fixed (byte* ptr = bytes)
        {
            var pointer = ptr;
            while (stringIndex < compareLength && comparisonResult == 0)
            {
                var charFromBytes = *pointer | (pointer[1] << 8);
                comparisonResult = a[stringIndex++] - charFromBytes;
                pointer += 2;
            }
        }
        if (comparisonResult != 0)
        {
            return comparisonResult;
        }

        return a.Length - bCharLength;
    }

    public static int CompareOrdinal(byte[] bytes, int aCharLength, string b)
    {
        return -CompareOrdinal(b, bytes, aCharLength);
    }

    public bool Equals(string a, string b)
    {
        return string.Equals(a, b);
    }

    public new bool Equals(object a, object b)
    {
        if (a == b)
        {
            return true;
        }
        var strA = (string)a;
        var strB = (string)b;

        return string.Equals(strA, strB);
    }

    public int GetHashCode(object key)
    {
        var stringKey = (string)key;

        return HashFunction(stringKey);
    }

    public int GetHashCode([DisallowNull] string key)
    {
        return HashFunction(key);
    }

    internal static unsafe int CompareOrdinal(byte* a, int byteLen, string b)
    {
        var comparisonResult = 0;
        var charIndex = 0;
        var charLength = byteLen >> 1;
        if (charLength > b.Length)
        {
            charLength = b.Length;
        }
        while (charIndex < charLength && comparisonResult == 0)
        {
            var currentChar = (char)(*a++ | (*a++ << 8));
            comparisonResult = currentChar - b[charIndex++];
        }
        if (comparisonResult != 0)
        {
            return comparisonResult;
        }

        return byteLen - b.Length * 2;
    }

    internal static int HashFunction(string key)
    {
        var hashValue = 5381u;
        for (var i = 0; i < key.Length; i++)
        {
            hashValue = ((hashValue << 5) + hashValue) ^ key[i];
        }

        return (int)hashValue;
    }
}