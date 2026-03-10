// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides methods to manage a collection of
    /// <see cref="T:System.Drawing.Image" /> objects. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    public sealed partial class ImageList
    {
        [Flags]
        private enum OriginalOptions
        {
            Default = 0x00,
            ImageStrip = 0x01,
            CustomTransparentColor = 0x02,
            OwnsImage = 0x04
        }
    }
}
