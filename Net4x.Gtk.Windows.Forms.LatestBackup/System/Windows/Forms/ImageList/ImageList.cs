// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GLib;

namespace System.Windows.Forms;

/// <summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
/// The ImageList is an object that stores a collection of Images, most
/// commonly used by other controls, such as the ListView, TreeView, or
/// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
/// other controls will be able to use the Images as they desire.
/// </summary>
[ToolboxItemFilter("System.Windows.Forms")]
[DefaultProperty(nameof(Images))]
[TypeConverter(typeof(ImageListConverter))]
public sealed partial class ImageList : Component //, IHandle<HIMAGELIST>
{
    /// <summary>
    /// The change handler
    /// </summary>
    private EventHandler? _changeHandler;

    /// <summary>
    /// The color depth
    /// </summary>
    private ColorDepth _colorDepth = ColorDepth.Depth32Bit;

    /// <summary>
    /// The image collection
    /// </summary>
    private ImageCollection? _imageCollection;

    /// <summary>
    /// The image size
    /// </summary>
    private Size _imageSize = s_defaultImageSize;

    /// <summary>
    /// The image stream
    /// </summary>
    private ImageListStreamer? _ImageStream;

    /// <summary>
    /// The native image list
    /// </summary>
    private NativeImageList? _nativeImageList;

    /// <summary>
    /// The originals
    /// </summary>
     // The usual handle virtualization problem, with a new twist: image
    // lists are lossy. At runtime, we delay handle creation as long as possible, and store
    // away the original images until handle creation (and hope no one disposes of the images!). At design time, we keep the originals around indefinitely.
    // This variable will become null when the original images are lost.
    private List<Original>? _originals = new List<Original>();

    /// <summary>
    /// The recreate handler
    /// </summary>
    private EventHandler? _recreateHandler;

    /// <summary>
    /// The s default image size
    /// </summary>
    private static readonly Size s_defaultImageSize = new Size(16, 16);

    /// <summary>
    /// The s fake transparency color
    /// </summary>
    private static readonly Color s_fakeTransparencyColor = Color.FromArgb(0x0d, 0x0b, 0x0c);

    /// <summary>
    /// The s is scaling initialized
    /// </summary>
    private static readonly bool s_isScalingInitialized;

    /// <summary>
    /// The s maximum image height
    /// </summary>
    private static readonly int s_maxImageHeight;

    /// <summary>
    /// The s maximum image width
    /// </summary>
    private static readonly int s_maxImageWidth;

    /// <summary>
    /// Occurs when [recreate handle.
    /// </summary>
    public event EventHandler? RecreateHandle { add => _recreateHandler += value; remove => _recreateHandler -= value; }

    /// <summary>
    /// Occurs when [change handle.
    /// </summary>
    internal event EventHandler? ChangeHandle { add => _changeHandler += value; remove => _changeHandler -= value; }

    /// <summary>
    /// Creates a new ImageList Control with a default image size of 16x16 pixels
    /// </summary>
    public ImageList()
    {
    }

    /// <summary>
    /// Creates a new ImageList Control with a default image size of 16x16
    /// pixels and adds the ImageList to the passed in container.
    /// </summary>
    /// <param name = "container">
    /// The container.
    /// </param>
    public ImageList(IContainer container) : this()
    {
        container.Add(this);
    }

    /// <summary>
    /// Gets or sets the color depth.
    /// </summary>
    /// <value>
    /// The color depth.
    /// </value>
    public ColorDepth ColorDepth
    {
        get => _colorDepth;
        set
        {
            if (_colorDepth == value)
            {
                return;
            }

            _colorDepth = value;
        }
    }

    /// <summary>
    /// Gets the handle.
    /// </summary>
    /// <value>
    /// The handle.
    /// </value>
    public IntPtr Handle
    {
        get
        {
            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// Gets a value indicating whether handle created.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if handle created; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool HandleCreated => !(_nativeImageList is null);

    /// <summary>
    /// Gets the images.
    /// </summary>
    /// <value>
    /// The images.
    /// </value>
    public ImageCollection Images => _imageCollection ??= new ImageCollection(this);

    /// <summary>
    /// Gets or sets the size of the image.
    /// </summary>
    /// <value>
    /// The size of the image.
    /// </value>
    public Size ImageSize
    {
        get => _imageSize;
        set
        {
            if (_imageSize.Width != value.Width || _imageSize.Height != value.Height)
            {
                _imageSize = new Size(value.Width, value.Height);
            }
        }
    }

    /// <summary>
    /// Gets or sets the image stream.
    /// </summary>
    /// <value>
    /// The image stream.
    /// </value>
    public ImageListStreamer? ImageStream
    {
        get
        {
            return _ImageStream;
        }

        set
        {
            _ImageStream = value;
            if (value is null)
            {
                Images.Clear();
            }
        }
    }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    /// <value>
    /// The tag.
    /// </value>

#if DEBUG

    internal bool IsDisposed { get; private set; }
#endif
    public object? Tag { get; set; }

    /// <summary>
    /// Gets or sets the color of the transparent.
    /// </summary>
    /// <value>
    /// The color of the transparent.
    /// </value>
    public Color TransparentColor { get; set; } = Color.Transparent;

    /// <summary>
    /// Gets a value indicating whether use transparent color.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use transparent color; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    private bool UseTransparentColor => TransparentColor.A > 0;

    /// <summary>
    /// Draw the image indicated by the given index on the given Graphics
    /// at the given location.
    /// </summary>
    /// <param name = "g">
    /// The g.
    /// </param>
    /// <param name = "pt">
    /// The pt.
    /// </param>
    /// <param name = "index">
    /// The index.
    /// </param>
    public void Draw(Graphics g, Point pt, int index) => Draw(g, pt.X, pt.Y, index);

    /// <summary>
    /// Draw the image indicated by the given index on the given Graphics
    /// at the given location.
    /// </summary>
    /// <param name = "g">
    /// The g.
    /// </param>
    /// <param name = "x">
    /// The x.
    /// </param>
    /// <param name = "y">
    /// The y.
    /// </param>
    /// <param name = "index">
    /// The index.
    /// </param>
    public void Draw(Graphics g, int x, int y, int index) => Draw(g, x, y, _imageSize.Width, _imageSize.Height, index);

    /// <summary>
    /// Draw the image indicated by the given index using the location, size
    /// and raster op code specified. The image is stretched or compressed as
    /// necessary to fit the bounds provided.
    /// </summary>
    /// <param name = "g">
    /// The g.
    /// </param>
    /// <param name = "x">
    /// The x.
    /// </param>
    /// <param name = "y">
    /// The y.
    /// </param>
    /// <param name = "width">
    /// The width.
    /// </param>
    /// <param name = "height">
    /// The height.
    /// </param>
    /// <param name = "index">
    /// The index.
    /// </param>
    public void Draw(Graphics g, int x, int y, int width, int height, int index)
    {
        //HDC dc = (HDC)g.GetHdc();
        //try
        //{
        //    PInvoke.ImageList.DrawEx(
        //        this,
        //        index,
        //        new HandleRef<HDC>(g, dc),
        //        x,
        //        y,
        //        width,
        //        height,
        //        (COLORREF)PInvoke.CLR_NONE,
        //        (COLORREF)PInvoke.CLR_NONE,
        //        IMAGE_LIST_DRAW_STYLE.ILD_TRANSPARENT);
        //}
        //finally
        //{
        //    g.ReleaseHdcInternal(dc);
        //}
    }

    /// <summary>
    /// Returns the image specified by the given index. The bitmap returned is a
    /// copy of the original image.
    /// </summary>
    /// <param name = "index">
    /// The index.
    /// </param>
    /// <returns>
    /// Bitmap.
    /// </returns>
    /// <exception cref="T:System.IndexOutOfRangeException">
    /// index out of range， please check whether the index is within the valid rangeImageList within the data range， save the related images toResources under the directory。
    /// </exception>
     // NOTE: forces handle creation, so doesn't return things from the original list
    public Bitmap GetBitmap(int index)
    {
        try
        {
            return _originals[index]._image as Bitmap;
        }
        catch (IndexOutOfRangeException ex)
        {
            throw new IndexOutOfRangeException(" index out of range， please check whether the index is within the valid rangeImageList within the data range， save the related images toResources under the directory。", ex);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    /// <summary>
    /// Gets the bitmap.
    /// </summary>
    /// <param name = "name">
    /// The name.
    /// </param>
    /// <returns>
    /// Bitmap.
    /// </returns>
    /// <exception cref="T:System.IO.FileNotFoundException">
    /// “{name}” not loaded， please save the related images toResources under the directory。
    /// </exception>
    public Bitmap GetBitmap(string name)
    {
        int index = _imageCollection.IndexOfKey(name);
        if (index == -1)
        {
            throw new FileNotFoundException($"“{name}” not loaded， please save the related images toResources under the directory。", name);
        }

        return GetBitmap(index);
    }

    /// <summary>
    /// Returns a
    /// <see cref="T:System.String"/> that represents this instance.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String"/> that represents this instance.
    /// </returns>
    public override string ToString() => Images is null ? base.ToString() : $"{base.ToString()} Images.Count: {Images.Count}, ImageSize: {ImageSize}";

    /// <summary>
    /// Gets the original image.
    /// </summary>
    /// <param name = "name">
    /// The name.
    /// </param>
    /// <returns>
    /// Image.
    /// </returns>
    internal Image GetOriginalImage(string name)
    {
        string path1 = $"./Resources";
        var value = this.ImageStream;
        if (value.ResourceInfo != null && value.ResourceInfo.BaseName != null)
        {
            // load image data here
            string dir = $"{path1}/{Path.GetExtension(value.ResourceInfo.BaseName).TrimStart('.')}";
            string path2 = $"{dir}/{name}";
            if (IO.File.Exists(path2))
            {
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return ScaleSimpleBitmap(Bitmap.FromFile(path2));
            }
        }

        if (IO.File.Exists($"{path1}/{name}"))
        {
            return ScaleSimpleBitmap(Bitmap.FromFile($"{path1}/{name}"));
        }

        return null;
    }

    /// <summary>
    /// Releases the unmanaged resources used by the
    /// <see cref="T:System.Windows.Forms.ImageList"/>
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name = "disposing">
    /// <see langword="true"/> to release both managed and unmanaged resources;
    /// <see langword="false"/> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (!(_originals is null))
            {
                // we might own some of the stuff that's not been created yet
                foreach (Original original in _originals)
                {
                    if ((original._options & OriginalOptions.OwnsImage) != 0)
                    {
                        ((IDisposable)original._image).Dispose();
                    }
                }

                _originals.Clear();
                _imageCollection.Clear();
            }

            DestroyHandle();
        }

#if DEBUG
        // At this stage we've released all resources, and the component is essentially disposed
        IsDisposed = true;
#endif
        base.Dispose(disposing);
    }

    /// <summary>
    /// Adds the icon to handle.
    /// </summary>
    /// <param name = "original">
    /// The original.
    /// </param>
    /// <param name = "icon">
    /// The icon.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    private int AddIconToHandle(Original original, Icon icon)
    {
        try
        {
            return 0;
        }
        finally
        {
        }
    }

    /// <summary>
    /// Adds to handle.
    /// </summary>
    /// <param name = "bitmap">
    /// The bitmap.
    /// </param>
    /// <returns>
    /// System.Int32.
    /// </returns>
    private int AddToHandle(Bitmap bitmap)
    {
        return 0;
    }

    /// <summary>
    /// Bitmaps the has alpha.
    /// </summary>
    /// <param name = "bmpData">
    /// The BMP data.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    private static unsafe bool BitmapHasAlpha(BitmapData bmpData)
    {
        if (bmpData.PixelFormat != PixelFormat.Format32bppArgb && bmpData.PixelFormat != PixelFormat.Format32bppRgb)
        {
            return false;
        }

        for (int i = 0; i < bmpData.Height; i++)
        {
            int offsetRow = i * bmpData.Stride;
            for (int j = 3; j < bmpData.Width * 4; j += 4) // *4 is safe since we know PixelFormat is ARGB
            {
                byte* candidate = ((byte*)bmpData.Scan0.ToPointer()) + offsetRow + j;
                if (*candidate != 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Copies the bitmap data.
    /// </summary>
    /// <param name = "sourceData">
    /// The source data.
    /// </param>
    /// <param name = "targetData">
    /// The target data.
    /// </param>
    private static unsafe void CopyBitmapData(BitmapData sourceData, BitmapData targetData)
    {
        Debug.Assert(Image.GetPixelFormatSize(sourceData.PixelFormat) == 32);
        Debug.Assert(Image.GetPixelFormatSize(sourceData.PixelFormat) == Image.GetPixelFormatSize(targetData.PixelFormat));
        Debug.Assert(targetData.Width == sourceData.Width);
        Debug.Assert(targetData.Height == sourceData.Height);
        Debug.Assert(targetData.Stride == targetData.Width * 4);
        // do the actual copy
        int offsetSrc = 0;
        int offsetDest = 0;
        for (int i = 0; i < targetData.Height; i++)
        {
            IntPtr srcPtr = sourceData.Scan0 + offsetSrc;
            IntPtr destPtr = targetData.Scan0 + offsetDest;
            int length = Math.Abs(targetData.Stride);
            Buffer.MemoryCopy(srcPtr.ToPointer(), destPtr.ToPointer(), length, length);
            offsetSrc += sourceData.Stride;
            offsetDest += targetData.Stride;
        }
    }

    /// <summary>
    /// Creates the bitmap.
    /// </summary>
    /// <param name = "original">
    /// The original.
    /// </param>
    /// <param name = "ownsBitmap">
    /// if set to
    /// <c>
    /// true
    /// </c>owns bitmap.
    /// </param>
    /// <returns>
    /// Bitmap.
    /// </returns>
    private Bitmap CreateBitmap(Original original, out bool ownsBitmap)
    {
        ownsBitmap = false;
        Bitmap bitmp = original._image as Bitmap;
        Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(bitmp.PixbufData);
        int w = Math.Max(16, Math.Min(ImageSize.Width, 200));
        int h = Math.Max(16, Math.Min(ImageSize.Height, 200));
        Gdk.Pixbuf newpixbuf = pixbuf.ScaleSimple(w, h, Gdk.InterpType.Bilinear);
        ownsBitmap = true;

        return new Bitmap(w, h)
        {
            Pixbuf = newpixbuf
        };
    }

    /// <summary>
    /// Creates the handle.
    /// </summary>
    private void CreateHandle()
    {
    }

    /// <summary>
    /// Destroys the handle.
    /// </summary>
     // Don't merge this function into Dispose() -- that base.Dispose() will damage the design time experience
    private void DestroyHandle()
    {
        if (HandleCreated)
        {
            _nativeImageList.Dispose();
            _nativeImageList = null;
            _originals = new List<Original>();
        }
    }

    /// <summary>
    /// Handles the
    /// <see cref="T:System.Windows.Forms.ImageList.ChangeHandle"/> event.
    /// </summary>
    /// <param name = "eventargs">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    private void OnChangeHandle(EventArgs eventargs) => _changeHandler?.Invoke(this, eventargs);

    /// <summary>
    /// Called when the Handle property changes.
    /// </summary>
    /// <param name = "eventargs">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    private void OnRecreateHandle(EventArgs eventargs) => _recreateHandler?.Invoke(this, eventargs);

    /// <summary>
    /// Resets the color depth.
    /// </summary>
    private void ResetColorDepth() => ColorDepth = ColorDepth.Depth32Bit;

    /// <summary>
    /// Scales the simple bitmap.
    /// </summary>
    /// <param name = "bitmp">
    /// The bitmp.
    /// </param>
    /// <returns>
    /// Bitmap.
    /// </returns>
    private Bitmap ScaleSimpleBitmap(Image bitmp)
    {
        Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(bitmp.PixbufData);
        int w = Math.Max(16, Math.Min(ImageSize.Width, 200));
        int h = Math.Max(16, Math.Min(ImageSize.Height, 200));
        Gdk.Pixbuf newpixbuf = pixbuf.ScaleSimple(w, h, Gdk.InterpType.Bilinear);

        return new Bitmap(w, h)
        {
            Pixbuf = newpixbuf
        };
    }

    /// <summary>
    /// Shoulds the serialize color depth.
    /// </summary>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    private bool ShouldSerializeColorDepth() => Images.Count == 0;

    /// <summary>
    /// Shoulds the size of the serialize image.
    /// </summary>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    private bool ShouldSerializeImageSize() => Images.Count == 0;

}