// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
public sealed partial class ImageList
{
    /// <summary>
    /// Class ImageCollection. This class cannot be inherited. Implements the
    /// <see cref="T:System.Collections.IList" />
    /// </summary>
    public sealed partial class ImageCollection
    {
        internal class ImageInfo
        {
            public string? Name { get; set; }

        }
    }
}