// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Represents an area within a <see cref="T:System.Windows.Forms.LinkLabel" /> that is a hyperlink.</summary>
public struct LinkArea
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.LinkArea" /> structure.</summary>
    /// <param name="start">The zero-based starting location of the link area within the text of the label.</param>
    /// <param name="length">The number of characters, after the start character, to include in the link area.</param>
    public LinkArea(int start, int length)
    {
        Start = start;
        Length = length;
    }

    /// <summary>Gets or sets the starting location of the link area within the text of the label.</summary>
    public int Start { get; set; }

    /// <summary>Gets or sets the number of characters in the link area.</summary>
    public int Length { get; set; }

    /// <summary>Gets a value indicating whether the link area is empty.</summary>
    public bool IsEmpty => Start == 0 && Length == 0;

    /// <summary>Determines whether the specified object is equal to this link area.</summary>
    /// <param name="o">The object to compare.</param>
    /// <returns><see langword="true" /> if equal; otherwise <see langword="false" />.</returns>
    public override bool Equals(object? o)
        => o is LinkArea a && a.Start == Start && a.Length == Length;

    /// <summary>Returns a string that represents this link area.</summary>
    /// <returns>A string describing the start and length.</returns>
    public override string ToString() => "{Start=" + Start + ", Length=" + Length + "}";

    /// <summary>Returns the hash code for this link area.</summary>
    /// <returns>A hash code.</returns>
    public override int GetHashCode() => Start ^ Length;

    /// <summary>Compares two link areas for equality.</summary>
    public static bool operator ==(LinkArea linkArea1, LinkArea linkArea2)
        => linkArea1.Start == linkArea2.Start && linkArea1.Length == linkArea2.Length;

    /// <summary>Compares two link areas for inequality.</summary>
    public static bool operator !=(LinkArea linkArea1, LinkArea linkArea2) => !(linkArea1 == linkArea2);
}
