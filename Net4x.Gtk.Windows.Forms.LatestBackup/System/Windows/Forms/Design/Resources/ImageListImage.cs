// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.IO;

namespace System.Windows.Forms.Design;

/// <summary>
/// Lightweight wrapper around a
/// <see cref="T:System.Windows.Forms.Design.ImageListImage.Image" /> used by image list editors and designers.
/// Exposes common image properties so instances appear like an
/// <see cref="T:System.Windows.Forms.Design.ImageListImage.Image" /> in collection editors.
/// </summary>
public class ImageListImage
{

    private string _name;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Design.ImageListImage" /> class that wraps the specified image.
    /// </summary>
    /// <param name="image">The image to wrap.
    /// </param>
    public ImageListImage(Image image)
    {
        Image = image;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Design.ImageListImage" /> class that wraps the specified image and assigns a name.
    /// </summary>
    /// <param name="image">The image to wrap.
    /// </param>
    /// <param name="name">The name to associate with the image.
    /// </param>
    public ImageListImage(Image image, string name)
    {
        Image = image;
        Name = name;
    }

    /// <summary>
    /// Gets the horizontal resolution, in dots per inch, of the wrapped image.
    /// </summary>
    // Add properties to make this object "look" like Image in the Collection editor
    public float HorizontalResolution => Image.HorizontalResolution;

    /// <summary>
    /// Gets or sets the underlying
    /// <see cref="T:System.Windows.Forms.Design.ImageListImage.Image" /> instance.
    /// </summary>
    [Browsable(false)]
    public Image Image { get; set; }

    /// <summary>
    /// Gets or sets the optional name associated with this image.
    /// Returns an empty string when no name is set.
    /// </summary>
    public string Name
    {
        get => _name ?? string.Empty;
        set => _name = value;
    }

    /// <summary>
    /// Gets the physical dimensions of the wrapped image.
    /// </summary>
    public SizeF PhysicalDimension => Image.Size;

    /// <summary>
    /// Gets the pixel format of the wrapped image.
    /// </summary>
    public PixelFormat PixelFormat => Image.PixelFormat;

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Drawing.Imaging.ImageFormat" /> of the wrapped image.
    /// </summary>
    public ImageFormat RawFormat => Image.RawFormat;

    /// <summary>
    /// Gets the size, in pixels, of the wrapped image.
    /// </summary>
    public Size Size => Image.Size;

    /// <summary>
    /// Gets the vertical resolution, in dots per inch, of the wrapped image.
    /// </summary>
    public float VerticalResolution => Image.VerticalResolution;

    /// <summary>
    /// Creates an
    /// <see cref="T:System.Windows.Forms.Design.ImageListImage" /> from a stream.
    /// If
    /// <paramref name="imageIsIcon" /> is true the stream is treated as an icon and converted to a bitmap.
    /// </summary>
    /// <param name="stream">Stream that contains image data.
    /// </param>
    /// <param name="imageIsIcon">If true, interpret the stream as an icon.
    /// </param>
    /// <returns>
    /// A new
    /// <see cref="T:System.Windows.Forms.Design.ImageListImage" /> instance wrapping the decoded image.
    /// </returns>
    public static ImageListImage ImageListImageFromStream(Stream stream, bool imageIsIcon)
    {
        if (imageIsIcon)
        {
            return new ImageListImage((new Icon(stream)).ToBitmap());
        }

        return new ImageListImage((Bitmap)Image.FromStream(stream));
    }

}