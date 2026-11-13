using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Gtk.Resources;

internal sealed class FastResourceComparer : IComparer, IEqualityComparer, IComparer<string>, IEqualityComparer<string>
{
    internal static readonly FastResourceComparer Default = new();

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
        var charIndex = 0;
        var charDiff = 0;
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
            var p = ptr;
            while (charIndex < compareLength && charDiff == 0)
            {
                var bChar = *p | (p[1] << 8);
                charDiff = a[charIndex++] - bChar;
                p += 2;
            }
        }
        if (charDiff != 0)
        {
            return charDiff;
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
        var a2 = (string)a;
        var b2 = (string)b;

        return string.Equals(a2, b2);
    }

    public int GetHashCode(object key)
    {
        var k = (string)key;

        return HashFunction(k);
    }

    public int GetHashCode([DisallowNull] string key)
    {
        return HashFunction(key);
    }

    internal static unsafe int CompareOrdinal(byte* a, int byteLen, string b)
    {
        var charDiff = 0;
        var charIndex = 0;
        var compareLength = byteLen >> 1;
        if (compareLength > b.Length)
        {
            compareLength = b.Length;
        }
        while (charIndex < compareLength && charDiff == 0)
        {
            var aChar = (char)(*(a++) | (*(a++) << 8));
            charDiff = aChar - b[charIndex++];
        }
        if (charDiff != 0)
        {
            return charDiff;
        }

        return byteLen - b.Length * 2;
    }

    internal static int HashFunction(string key)
    {
        var hash = 5381u;
        for (var i = 0; i < key.Length; i++)
        {
            hash = ((hash << 5) + hash) ^ key[i];
        }

        return (int)hash;
    }
}