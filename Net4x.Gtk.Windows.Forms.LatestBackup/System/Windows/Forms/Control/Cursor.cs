// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using GLib;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents the image used to paint the mouse pointer.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    /// <completionlist cref="System.Windows.Forms.Cursors" />
    public sealed class Cursor : IDisposable, ISerializable
    {

        private readonly byte[]? _cursorData;

        private readonly bool _freeHandle;

        private IntPtr _handle;

        private static Size s_cursorSize = Size.Empty;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Cursor" /> class from the specified Windows handle.
        /// </summary>
        /// <param name="handle">An
        /// <see cref="System.IntPtr" /> that represents the Windows handle of the cursor to create.
        /// </param>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="handle" /> is
        /// <see cref="System.IntPtr.Zero" />
        /// </exception>
        public Cursor(IntPtr handle)
        {
            _freeHandle = false;
            _handle = handle;
            var cur = new Gdk.Cursor(handle);
            _cursorData = cur.Image.ReadPixelBytes().Data;
        }

        public Cursor(Gdk.CursorType cursorType)
        {
            _freeHandle = false;
            CursorType = cursorType;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Cursor" /> class from the specified file.
        /// </summary>
        /// <param name="fileName">The cursor file to load.
        /// </param>
        public Cursor(string fileName)
        {
            _cursorData = File.ReadAllBytes(fileName);
            _freeHandle = true;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Cursor" /> class from the specified resource with the specified resource type.
        /// </summary>
        /// <param name="type">The resource
        /// <see cref="System.Type" />
        /// </param>
        /// <param name="resource">The name of the resource.
        /// </param>
        public Cursor(Type type, string resource) : this(type.Module.Assembly.GetManifestResourceStream(resource)!)
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.Cursor" /> class from the specified data stream.
        /// </summary>
        /// <param name="stream">The data stream to load the
        /// <see cref="System.Windows.Forms.Cursor" /> from.
        /// </param>
        public Cursor(Stream stream)
        {
            _freeHandle = true;
            CursorType = Gdk.CursorType.CursorIsPixmap;
            CursorPixbuf = new Gdk.Pixbuf(stream).ScaleSimple(48, 48, Gdk.InterpType.Tiles);
        }
        internal Cursor(string resource, string cursorsProperty)
            : this(typeof(Cursors).Assembly.GetManifestResourceStream(resource))
        {
            CursorsProperty = cursorsProperty;
            _freeHandle = false;
        }

        internal Cursor(string resource, int x, int y)
            : this(typeof(Cursors).Assembly.GetManifestResourceStream(resource))
        {
            CursorsXY = new Point(x, y);
            _freeHandle = false;
        }
        /// <summary>
        /// Gets or sets the bounds that represent the clipping rectangle for the cursor.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Rectangle" /> that represents the clipping rectangle for the
        /// <see cref="System.Windows.Forms.Cursor" />, in screen coordinates.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static unsafe Rectangle Clip
        {
            get
            {
                return new Rectangle(0, 0, 16, 16);
            }
            set
            {

            }
        }

        /// <summary>
        /// Gets or sets a cursor object that represents the mouse cursor.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.Cursor" /> that represents the mouse cursor. The default is null if the mouse cursor is not visible.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Cursor? Current
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the handle of the cursor.
        /// </summary>
        /// <returns>An
        /// <see cref="System.IntPtr" /> that represents the cursor's handle.
        /// </returns>
        /// <exception cref="System.Exception">The handle value is
        /// <see cref="System.IntPtr.Zero" />
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public IntPtr Handle
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Gets the cursor hot spot.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" /> representing the cursor hot spot.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Point HotSpot
        {
            get;
        }

        /// <summary>
        /// Gets or sets the cursor's position.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Point" /> that represents the cursor's position in screen coordinates.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public static Point Position
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the size of the cursor object.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Size" /> that represents the width and height of the
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Size Size
        {
            get;
        }

        /// <summary>
        /// Gets or sets the object that contains data about the
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </summary>
        /// <returns>An
        /// <see cref="System.Object" /> that contains data about the
        /// <see cref="System.Windows.Forms.Cursor" /> The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object? Tag { get; set; }

        internal Gdk.Pixbuf CursorPixbuf { get; set; }

        internal string? CursorsProperty { get; }

        internal Point CursorsXY { get; }

        internal Gdk.CursorType CursorType { get; set; }
        /// <summary>
        /// Copies the handle of this
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </summary>
        /// <returns>An
        /// <see cref="System.IntPtr" /> that represents the cursor's handle.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IntPtr CopyHandle()
        {
            return IntPtr.Zero;
        }

        /// <summary>
        /// Releases all resources used by the
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Draws the cursor on the specified surface, within the specified bounds.
        /// </summary>
        /// <param name="g">The
        /// <see cref="System.Drawing.Graphics" /> surface on which to draw the
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </param>
        /// <param name="targetRect">The
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounds of the
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void Draw(Graphics g, Rectangle targetRect)
        {

        }

        /// <summary>
        /// Draws the cursor in a stretched format on the specified surface, within the specified bounds.
        /// </summary>
        /// <param name="g">The
        /// <see cref="System.Drawing.Graphics" /> surface on which to draw the
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </param>
        /// <param name="targetRect">The
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounds of the
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public void DrawStretched(Graphics g, Rectangle targetRect)
        {

        }

        /// <summary>
        /// Returns a value indicating whether this cursor is equal to the specified
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </summary>
        /// <returns>true if this cursor is equal to the specified
        /// <see cref="System.Windows.Forms.Cursor" />; otherwise, false.
        /// </returns>
        /// <param name="obj">The
        /// <see cref="System.Windows.Forms.Cursor" /> to compare.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool Equals(object? obj) => obj is Cursor cursor && this == cursor;

        /// <summary>
        /// Retrieves the hash code for the current
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </summary>
        /// <returns>A hash code for the current
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override int GetHashCode()
        {
            if (_cursorData != null)
            {
                return _cursorData.GetHashCode();
            }
            else if (_handle != IntPtr.Zero)
            {
                return _handle.GetHashCode();
            }
            else
                return 0;

        }

        /// <summary>
        /// Hides the cursor.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public static void Hide() { }

        /// <summary>
        /// Displays the cursor.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        public static void Show() { }

        /// <summary>
        /// Retrieves a human readable string representing this
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.String" /> that represents this
        /// <see cref="System.Windows.Forms.Cursor" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string ToString() => $"[Cursor: {CursorsProperty ?? base.ToString()}]";

        internal unsafe byte[] GetData()
        {
            return (byte[])_cursorData.Clone();
        }
        /// <summary>
        /// Serializes the object.
        /// </summary>
        /// <param name="si">A
        /// <see cref="System.Runtime.Serialization.SerializationInfo" /> class.
        /// </param>
        /// <param name="context">A
        /// <see cref="System.Runtime.Serialization.StreamingContext" /> class.
        /// </param>
        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }

        private void Dispose(bool disposing)
        {

        }
    }
}
