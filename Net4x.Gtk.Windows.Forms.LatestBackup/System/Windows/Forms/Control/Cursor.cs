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

        public Cursor(string fileName)
        {
            _cursorData = File.ReadAllBytes(fileName);
            _freeHandle = true;
        }

        public Cursor(Type type, string resource) : this(type.Module.Assembly.GetManifestResourceStream(resource)!)
        {
        }

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

        public static Cursor? Current
        {
            get;
            set;
        }

        public IntPtr Handle
        {
            get
            {
                return IntPtr.Zero;
            }
        }

        public Point HotSpot
        {
            get;
        }

        public static Point Position
        {
            get;
            set;
        }

        public Size Size
        {
            get;
        }

        public object? Tag { get; set; }

        internal Gdk.Pixbuf CursorPixbuf { get; set; }

        internal string? CursorsProperty { get; }

        internal Point CursorsXY { get; }

        internal Gdk.CursorType CursorType { get; set; }

        public IntPtr CopyHandle()
        {
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Draw(Graphics g, Rectangle targetRect)
        {

        }

        public void DrawStretched(Graphics g, Rectangle targetRect)
        {

        }

        public override bool Equals(object? obj) => obj is Cursor cursor && this == cursor;

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

        public static void Hide() { }

        public static void Show() { }

        public override string ToString() => $"[Cursor: {CursorsProperty ?? base.ToString()}]";

        internal unsafe byte[] GetData()
        {
            return (byte[])_cursorData.Clone();
        }

        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            throw new PlatformNotSupportedException();
        }

        private void Dispose(bool disposing)
        {

        }
    }
}
