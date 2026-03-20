// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using GLib;

namespace System.Windows.Forms;

/// <summary>
/// Represents the image used to paint the mouse pointer.
/// </summary>
/// <filterpriority>
/// 1
/// </filterpriority>
/// <completionlist cref="T:System.Windows.Forms.Cursors" />
public sealed class Cursor : IDisposable, ISerializable
{

    private readonly byte[]? _cursorData;

    private readonly bool _freeHandle;

    private readonly IntPtr _handle;

    private static Size s_cursorSize = Size.Empty;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Cursor" /> class from a native handle.
    /// </summary>
    /// <param name="handle">
    /// Native cursor handle.
    /// </param>
    public Cursor(IntPtr handle)
    {
        _freeHandle = false;
        _handle = handle;
        var cur =new Gdk.Cursor(handle);
        _cursorData = cur.Image.ReadPixelBytes().Data;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Cursor" /> class for a standard GDK cursor type.
    /// </summary>
    /// <param name="cursorType">
    /// The GDK cursor type.
    /// </param>
    public Cursor(Gdk.CursorType cursorType)
    {
        _freeHandle = false;
        CursorType = cursorType;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Cursor" /> class from a cursor file.
    /// </summary>
    /// <param name="fileName">
    /// Path to the cursor file.
    /// </param>
    public Cursor(string fileName)
    {
        _cursorData = File.ReadAllBytes(fileName);
        _freeHandle = true;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Cursor" /> class from an embedded resource.
    /// </summary>
    /// <param name="type">
    /// Type used to locate the assembly containing the resource.
    /// </param>
    /// <param name="resource">
    /// Resource name.
    /// </param>
    public Cursor(Type type, string resource) : this(type.Module.Assembly.GetManifestResourceStream(resource)!)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.Cursor" /> class from a stream.
    /// </summary>
    /// <param name="stream">
    /// Stream containing cursor image data.
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
    /// Gets or sets the cursor clip rectangle. On this platform this property is a placeholder and returns a default rectangle.
    /// </summary>
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
    /// Gets or sets the current cursor instance.
    /// </summary>
    public static Cursor? Current
    {
        get;

        set;
    }

    /// <summary>
    /// Gets the underlying native cursor handle.
    /// </summary>
    public IntPtr Handle
    {
        get
        {
            return IntPtr.Zero;
        }
    }

    /// <summary>
    /// Gets the hotspot (active point) of the cursor.
    /// </summary>
    public Point HotSpot
    {
        get;
    }

    /// <summary>
    /// Gets or sets the current cursor position in screen coordinates.
    /// </summary>
    public static Point Position
    {
        get;

        set;
    }

    /// <summary>
    /// Gets the size of the cursor.
    /// </summary>
    public Size Size
    {
        get;
    }

    /// <summary>
    /// Gets or sets optional user data associated with the cursor.
    /// </summary>
    public object? Tag
    {
        get;
        set;
    }

    internal Gdk.Pixbuf CursorPixbuf
    {
        get;
        set;
    }

    internal string? CursorsProperty { get; }

    internal Point CursorsXY { get; }

    internal Gdk.CursorType CursorType
    {
        get;
        set;
    }

    /// <summary>
    /// Returns a copy of the underlying native handle.
    /// </summary>
    /// <returns>
    /// A copy of the native handle.
    /// </returns>
    public IntPtr CopyHandle()
    {
        return IntPtr.Zero;
    }

    /// <summary>
    /// Releases resources used by the cursor.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    /// Draws the cursor into the specified graphics surface without scaling.
    /// </summary>
    /// <param name="g">
    /// Graphics surface to draw into.
    /// </param>
    /// <param name="targetRect">
    /// Target rectangle in which to draw the cursor.
    /// </param>
    public void Draw(Graphics g, Rectangle targetRect)
    {

    }

    /// <summary>
    /// Draws the cursor stretched to fit the specified target rectangle.
    /// </summary>
    /// <param name="g">
    /// Graphics surface to draw into.
    /// </param>
    /// <param name="targetRect">
    /// Target rectangle in which to draw the cursor.
    /// </param>
    public void DrawStretched(Graphics g, Rectangle targetRect)
    {

    }

    /// <summary>
    /// Compares this instance to another object for equality.
    /// </summary>
    /// <param name="obj">
    /// Object to compare with.
    /// </param>
    /// <returns>
    /// True when equal; otherwise false.
    /// </returns>
    public override bool Equals(object? obj) => obj is Cursor cursor && this == cursor;

    /// <summary>
    /// Returns a hash code for the cursor instance.
    /// </summary>
    /// <returns>
    /// An integer hash code.
    /// </returns>
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
    /// Hides the system cursor (placeholder).
    /// </summary>
    public static void Hide() { }

    /// <summary>
    /// Shows the system cursor (placeholder).
    /// </summary>
    public static void Show() { }

    /// <summary>
    /// Returns a string that represents the current cursor.
    /// </summary>
    /// <returns>
    /// A string representation.
    /// </returns>
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