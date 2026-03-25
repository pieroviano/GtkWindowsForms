//
// System.Windows.Drawing.CharacterRange.cs
//
// Authors:
//	Dennis Hayes (dennish@raytek.com)
//	Sebastien Pouliot  <sebastien@ximian.com>
//
// Copyright (C) 2002 Ximian, Inc http://www.ximian.com
// Copyright (C) 2004, 2008 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

namespace System.Drawing;

/// <summary>
/// Specifies a range of character positions within a string.
/// </summary>
public struct CharacterRange
{
    private int first;

    private int length;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.CharacterRange" /> structure, specifying a range of character positions within a string.
    /// </summary>
    /// <param name="First">
    /// The position of the first character in the range. For example, if
    /// <paramref name="First" /> is set to 0, the first position of the range is position 0 in the string.
    /// </param>
    /// <param name="Length">
    /// The number of positions in the range.
    /// </param>
    public CharacterRange(int First, int Length)
    {
        this.first = First;
        this.length = Length;
    }

    /// <summary>
    /// Gets or sets the position in the string of the first character of this
    /// <see cref="T:System.Drawing.CharacterRange" />.
    /// </summary>
    /// <returns>
    /// The first position of this
    /// <see cref="T:System.Drawing.CharacterRange" />.
    /// </returns>
    public int First
    {
        get
        {
            return first;
        }
        set
        {
            first = value;
        }
    }

    /// <summary>
    /// Gets or sets the number of positions in this
    /// <see cref="T:System.Drawing.CharacterRange" />.
    /// </summary>
    /// <returns>
    /// The number of positions in this
    /// <see cref="T:System.Drawing.CharacterRange" />.
    /// </returns>
    public int Length
    {
        get
        {
            return length;
        }
        set
        {
            length = value;
        }
    }

    /// <summary>
    /// Gets a value indicating whether this object is equivalent to the specified object.
    /// </summary>
    /// <param name="obj">
    /// The object to compare to for equality.
    /// </param>
    /// <returns>
    /// <see langword="true" /> to indicate the specified object is an instance with the same
    /// <see cref="T:System.Drawing.CharacterRange.First" /> and
    /// <see cref="T:System.Drawing.CharacterRange.Length" /> value as this instance; otherwise,
    /// <see langword="false" />.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (!(obj is CharacterRange))
            return false;

        var cr = (CharacterRange) obj;

        return Equals(this, cr);
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer that is the hash code for this instance.
    /// </returns>
    public override int GetHashCode()
    {
        return (first ^ length);
    }
}