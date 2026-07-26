// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;
using System.Drawing;

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
    /// An image before we add it to the image list, along with a few details about how to add it.
    /// </summary>
    private class Original
    {
        internal Color _customTransparentColor = Color.Transparent;

        internal object _image;

        internal int _nImages = 1;

        internal OriginalOptions _options;

        internal Original(object image, OriginalOptions options) : this(image, options, Color.Transparent)
        {
        }

        internal Original(object image, OriginalOptions options, int nImages) : this(image, options, Color.Transparent)
        {
            _nImages = nImages;
        }

        internal Original(object image, OriginalOptions options, Color customTransparentColor)
        {

            _image = image;
            _options = options;
            _customTransparentColor = customTransparentColor;
            if ((options & OriginalOptions.CustomTransparentColor) == 0)
            {
                Debug.Assert(customTransparentColor.Equals(Color.Transparent), "Specified a custom transparent color then told us to ignore it");
            }
        }
    }
}