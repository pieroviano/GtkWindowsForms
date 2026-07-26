// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>Provides data for the <see cref="!:ListView.SearchForVirtualItem" /> event.</summary>
public class SearchForVirtualItemEventArgs : EventArgs
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.SearchForVirtualItemEventArgs" /> class.</summary>
    /// <param name="isTextSearch">Whether the search is by text.</param>
    /// <param name="isPrefixSearch">Whether the search matches a prefix.</param>
    /// <param name="includeSubItemsInSearch">Whether subitems are searched.</param>
    /// <param name="text">The text to search for.</param>
    /// <param name="startingPoint">The point at which to start the search.</param>
    /// <param name="direction">The direction of the search.</param>
    /// <param name="startIndex">The index at which to start the search.</param>
    public SearchForVirtualItemEventArgs(bool isTextSearch, bool isPrefixSearch, bool includeSubItemsInSearch, string text, Point startingPoint, SearchDirectionHint direction, int startIndex)
    {
        IsTextSearch = isTextSearch;
        IsPrefixSearch = isPrefixSearch;
        IncludeSubItemsInSearch = includeSubItemsInSearch;
        Text = text;
        StartingPoint = startingPoint;
        Direction = direction;
        StartIndex = startIndex;
        Index = -1;
    }

    /// <summary>Gets a value indicating whether the search is by text.</summary>
    public bool IsTextSearch { get; }

    /// <summary>Gets a value indicating whether subitems are included in the search.</summary>
    public bool IncludeSubItemsInSearch { get; }

    /// <summary>Gets or sets the index of the item found by the search.</summary>
    public int Index { get; set; }

    /// <summary>Gets a value indicating whether the search matches a prefix.</summary>
    public bool IsPrefixSearch { get; }

    /// <summary>Gets the text to search for.</summary>
    public string Text { get; }

    /// <summary>Gets the point at which to start the search.</summary>
    public Point StartingPoint { get; }

    /// <summary>Gets the direction of the search.</summary>
    public SearchDirectionHint Direction { get; }

    /// <summary>Gets the index at which to start the search.</summary>
    public int StartIndex { get; }
}
