// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides methods to manage a collection of
    /// <see cref="System.Drawing.Image" /> objects. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    public sealed partial class ImageList
    {
        /// <summary>
        /// Encapsulates the collection of
        /// <see cref="System.Drawing.Image" />
        /// objects in an
        /// <see cref="System.Windows.Forms.ImageList" />
        /// </summary>
        public sealed partial class ImageCollection
        {
            internal class ImageInfo
            {
                public string? Name { get; set; }
            }
        }
    }
}
