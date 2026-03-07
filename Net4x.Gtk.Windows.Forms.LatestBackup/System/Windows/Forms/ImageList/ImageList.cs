// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using GLib;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.Windows.Forms
{
    /// <summary>
    /// The ImageList is an object that stores a collection of Images, most
    /// commonly used by other controls, such as the ListView, TreeView, or
    /// Toolbar. You can add either bitmaps or Icons to the ImageList, and the
    /// other controls will be able to use the Images as they desire.
    /// </summary>
    /// <filterpriority>2
    /// </filterpriority>
    [ToolboxItemFilter("System.Windows.Forms")]
    [DefaultProperty(nameof(Images))]
    [TypeConverter(typeof(ImageListConverter))]
    public sealed partial class ImageList : Component //, IHandle<HIMAGELIST>
    {
        private EventHandler? _changeHandler;

        private ColorDepth _colorDepth = ColorDepth.Depth32Bit;

        private ImageCollection? _imageCollection;

        private Size _imageSize = s_defaultImageSize;

        private ImageListStreamer? _ImageStream;

        private NativeImageList? _nativeImageList;

        // The usual handle virtualization problem, with a new twist: image
        // lists are lossy. At runtime, we delay handle creation as long as possible, and store
        // away the original images until handle creation (and hope no one disposes of the images!). At design time, we keep the originals around indefinitely.
        // This variable will become null when the original images are lost.
        private List<Original>? _originals = new List<Original>();

        private EventHandler? _recreateHandler;

        private static readonly Size s_defaultImageSize = new Size(16, 16);

        private static readonly Color s_fakeTransparencyColor = Color.FromArgb(0x0d, 0x0b, 0x0c);

        private static bool s_isScalingInitialized;

        private static int s_maxImageHeight;

        private static int s_maxImageWidth;

        public event EventHandler? RecreateHandle { add => _recreateHandler += value; remove => _recreateHandler -= value; }

        internal event EventHandler? ChangeHandle { add => _changeHandler += value; remove => _changeHandler -= value; }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref = "System.Windows.Forms.ImageList"/> class with default values for
        /// <see cref = "System.Windows.Forms.ImageList.ColorDepth"/>,
        /// <see cref = "System.Windows.Forms.ImageList.ImageSize"/>, and
        /// <see cref = "System.Windows.Forms.ImageList.TransparentColor"/>
        /// </summary>
        public ImageList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref = "System.Windows.Forms.ImageList"/> class, associating it with a container.
        /// </summary>
        /// <param name = "container">An object implementing
        /// <see cref = "System.ComponentModel.IContainer"/> to associate with this instance of
        /// <see cref = "System.Windows.Forms.ImageList"/>
        /// </param>
        public ImageList(IContainer container) : this()
        {
            container.Add(this);
        }

        /// <summary>
        /// Gets the color depth of the image list.
        /// </summary>
        /// <returns>The number of available colors for the image. In the .NET Framework version 1.0, the default is
        /// <see cref = "System.Windows.Forms.ColorDepth.Depth4Bit"/> In the .NET Framework version 1.1 or later, the default is
        /// <see cref = "System.Windows.Forms.ColorDepth.Depth8Bit"/>
        /// </returns>
        /// <exception cref = "System.ComponentModel.InvalidEnumArgumentException">The color depth is not a valid
        /// <see cref = "System.Windows.Forms.ColorDepth"/> enumeration value.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
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
        /// Gets the handle of the image list object.
        /// </summary>
        /// <returns>The handle for the image list. The default is null.
        /// </returns>
        /// <exception cref = "System.InvalidOperationException">Creating the handle for the
        /// <see cref = "System.Windows.Forms.ImageList"/> failed.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public IntPtr Handle
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the underlying Win32 handle has been created.
        /// </summary>
        /// <returns>true if the
        /// <see cref = "System.Windows.Forms.ImageList.Handle"/> has been created; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool HandleCreated => !(_nativeImageList is null);

        /// <summary>
        /// Gets the
        /// <see cref = "System.Windows.Forms.ImageList.ImageCollection"/> for this image list.
        /// </summary>
        /// <returns>The collection of images.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public ImageCollection Images => _imageCollection ??= new ImageCollection(this);

        /// <summary>
        /// Gets or sets the size of the images in the image list.
        /// </summary>
        /// <returns>The
        /// <see cref = "System.Drawing.Size"/> that defines the height and width, in pixels, of the images in the list. The default size is 16 by 16. The maximum size is 256 by 256.
        /// </returns>
        /// <exception cref = "System.ArgumentException">The value assigned is equal to
        /// <see cref = "System.Drawing.Size.IsEmpty"/>-or- The value of the height or width is less than or equal to 0.-or- The value of the height or width is greater than 256.
        /// </exception>
        /// <exception cref = "System.ArgumentOutOfRangeException">The new size has a dimension less than 0 or greater than 256.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
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
        /// Gets the
        /// <see cref = "System.Windows.Forms.ImageListStreamer"/> associated with this image list.
        /// </summary>
        /// <returns>null if the image list is empty; otherwise, a
        /// <see cref = "System.Windows.Forms.ImageListStreamer"/> for this
        /// <see cref = "System.Windows.Forms.ImageList"/>
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
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
        /// Gets or sets an object that contains additional data about the
        /// <see cref = "System.Windows.Forms.ImageList"/>
        /// </summary>
        /// <returns>An
        /// <see cref = "System.Object"/> that contains additional data about the
        /// <see cref = "System.Windows.Forms.ImageList"/>
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>

#if DEBUG

        internal bool IsDisposed { get; private set; }
#endif
        public object? Tag { get; set; }

        /// <summary>
        /// Gets or sets the color to treat as transparent.
        /// </summary>
        /// <returns>One of the
        /// <see cref = "System.Drawing.Color"/> values. The default is Transparent.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color TransparentColor { get; set; } = Color.Transparent;
        private bool UseTransparentColor => TransparentColor.A > 0;

        /// <summary>
        /// Draws the image indicated by the specified index on the specified
        /// <see cref = "System.Drawing.Graphics"/> at the given location.
        /// </summary>
        /// <param name = "g">The
        /// <see cref = "System.Drawing.Graphics"/> to draw on.
        /// </param>
        /// <param name = "pt">The location defined by a
        /// <see cref = "System.Drawing.Point"/> at which to draw the image.
        /// </param>
        /// <param name = "index">The index of the image in the
        /// <see cref = "System.Windows.Forms.ImageList"/> to draw.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The index is less than 0.-or- The index is greater than or equal to the count of images in the image list.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void Draw(Graphics g, Point pt, int index) => Draw(g, pt.X, pt.Y, index);

        /// <summary>
        /// Draws the image indicated by the given index on the specified
        /// <see cref = "System.Drawing.Graphics"/> at the specified location.
        /// </summary>
        /// <param name = "g">The
        /// <see cref = "System.Drawing.Graphics"/> to draw on.
        /// </param>
        /// <param name = "x">The horizontal position at which to draw the image.
        /// </param>
        /// <param name = "y">The vertical position at which to draw the image.
        /// </param>
        /// <param name = "index">The index of the image in the
        /// <see cref = "System.Windows.Forms.ImageList"/> to draw.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The index is less than 0.-or- The index is greater than or equal to the count of images in the image list.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void Draw(Graphics g, int x, int y, int index) => Draw(g, x, y, _imageSize.Width, _imageSize.Height, index);

        /// <summary>
        /// Draws the image indicated by the given index on the specified
        /// <see cref = "System.Drawing.Graphics"/> using the specified location and size.
        /// </summary>
        /// <param name = "g">The
        /// <see cref = "System.Drawing.Graphics"/> to draw on.
        /// </param>
        /// <param name = "x">The horizontal position at which to draw the image.
        /// </param>
        /// <param name = "y">The vertical position at which to draw the image.
        /// </param>
        /// <param name = "width">The width, in pixels, of the destination image.
        /// </param>
        /// <param name = "height">The height, in pixels, of the destination image.
        /// </param>
        /// <param name = "index">The index of the image in the
        /// <see cref = "System.Windows.Forms.ImageList"/> to draw.
        /// </param>
        /// <exception cref = "System.ArgumentOutOfRangeException">The index is less than 0.-or- The index is greater than or equal to the count of images in the image list.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
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
        /// Returns a string that represents the current
        /// <see cref = "System.Windows.Forms.ImageList"/>
        /// </summary>
        /// <returns>A string that represents the current
        /// <see cref = "System.Windows.Forms.ImageList"/>
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public override string ToString() => Images is null ? base.ToString() : $"{base.ToString()} Images.Count: {Images.Count}, ImageSize: {ImageSize}";

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
        /// <see cref = "ImageList"/>
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

        private int AddToHandle(Bitmap bitmap)
        {
            return 0;
        }

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

        private void CreateHandle()
        {
        }

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

        private void OnChangeHandle(EventArgs eventargs) => _changeHandler?.Invoke(this, eventargs);

        /// <summary>
        /// Called when the Handle property changes.
        /// </summary>
        private void OnRecreateHandle(EventArgs eventargs) => _recreateHandler?.Invoke(this, eventargs);

        private void ResetColorDepth() => ColorDepth = ColorDepth.Depth32Bit;

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

        private bool ShouldSerializeColorDepth() => Images.Count == 0;

        private bool ShouldSerializeImageSize() => Images.Count == 0;
    }
}
