// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies which imagelist a property relates to. For example ImageListIndex must relate to a
    /// specific ImageList property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RelatedImageListAttribute : Attribute
    {
        private readonly string? _relatedImageList;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.RelatedImageListAttribute" /> class.
        /// </summary>
        /// <param name="relatedImageList">The name of the
        /// <see cref="System.Windows.Forms.ImageList" /> the property relates to.
        /// </param>
        public RelatedImageListAttribute(string? relatedImageList)
        {
            _relatedImageList = relatedImageList;
        }

        /// <summary>
        /// Gets the name of the related
        /// <see cref="System.Windows.Forms.ImageList" />
        /// </summary>
        /// <returns>The name of the related
        /// <see cref="System.Windows.Forms.ImageList" />
        /// </returns>
        public string? RelatedImageList
        {
            get
            {
                return _relatedImageList;
            }
        }

    }
}
