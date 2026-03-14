// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Specifies which imagelist a property relates to. For example ImageListIndex must relate to a
/// specific ImageList property
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public sealed class RelatedImageListAttribute : Attribute
{
    /// <summary>
    /// The related image list
    /// </summary>
    private readonly string? _relatedImageList;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.RelatedImageListAttribute" /> class.
    /// </summary>
    /// <param name="relatedImageList">
    /// The related image list.
    /// </param>
    public RelatedImageListAttribute(string? relatedImageList)
    {
        _relatedImageList = relatedImageList;
    }

    /// <summary>
    /// Gets the related image list.
    /// </summary>
    /// <value>
    /// The related image list.
    /// </value>
    public string? RelatedImageList
    {
        get
        {
            return _relatedImageList;
        }
    }

}