// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// This event is fired by owner drawn
/// <see cref="T:System.Windows.Forms.Control" />
/// objects, such as
/// <see cref="T:System.Windows.Forms.ListBox" /> and
/// <see cref="T:System.Windows.Forms.ComboBox" />
/// It contains all the information needed for the user to paint the given item,
/// including the item index, the
/// <see cref="T:System.Drawing.Rectangle" />
/// in which the drawing should be done, and the
/// <see cref="T:System.Windows.Forms.DrawItemEventArgs.Graphics" />
/// object with which the drawing should be done.
/// </summary>
public class DrawItemEventArgs : EventArgs, IDisposable, IDeviceContext
{

    /// <summary>
    /// The backColor to paint each menu item with.
    /// </summary>
    private readonly Color _backColor;

    /// <summary>
    /// The foreColor to paint each menu item with.
    /// </summary>
    private readonly Color _foreColor;

    private readonly Graphics graphics;

    /// <summary>
    /// Creates a new DrawItemEventArgs with the given parameters.
    /// </summary>
    /// <param name="graphics">
    /// The graphics.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    /// <param name="rect">
    /// The rect.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="state">
    /// The state.
    /// </param>
    public DrawItemEventArgs(Graphics graphics, Font? font, Rectangle rect, int index, DrawItemState state)
        : this(graphics, font, rect, index, state, SystemColors.WindowText, SystemColors.Window)
    { }

    /// <summary>
    /// Creates a new DrawItemEventArgs with the given parameters, including the foreColor and backColor
    /// of the control.
    /// </summary>
    /// <param name="graphics">
    /// The graphics.
    /// </param>
    /// <param name="font">
    /// The font.
    /// </param>
    /// <param name="rect">
    /// The rect.
    /// </param>
    /// <param name="index">
    /// The index.
    /// </param>
    /// <param name="state">
    /// The state.
    /// </param>
    /// <param name="foreColor">
    /// Color of the fore.
    /// </param>
    /// <param name="backColor">
    /// Color of the back.
    /// </param>
    /// <exception cref="T:System.ArgumentNullException">
    /// graphics
    /// </exception>
    public DrawItemEventArgs(
        Graphics graphics,
        Font? font,
        Rectangle rect,
        int index,
        DrawItemState state,
        Color foreColor,
        Color backColor)
    {
        this.graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
        this.Bounds = rect;
        Font = font;
        Index = index;
        State = state;
        _foreColor = foreColor;
        _backColor = backColor;
    }

    /// <summary>
    /// Gets the color of the back.
    /// </summary>
    /// <value>
    /// The color of the back.
    /// </value>
    public Color BackColor
                => (State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.Highlight : _backColor;

    /// <summary>
    /// The rectangle outlining the area in which the painting should be  done.
    /// </summary>
    /// <value>
    /// The bounds.
    /// </value>
    public Rectangle Bounds { get; }

    /// <summary>
    /// A suggested font, usually the parent control's Font property.
    /// </summary>
    /// <value>
    /// The font.
    /// </value>
    public Font? Font { get; }

    /// <summary>
    /// A suggested color drawing: either SystemColors.WindowText or SystemColors.HighlightText,
    /// depending on whether this item is selected.
    /// </summary>
    /// <value>
    /// The color of the fore.
    /// </value>
    public Color ForeColor
        => (State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : _foreColor;

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Drawing.Graphics" /> object used to paint.
    /// </summary>
    /// <value>
    /// The graphics.
    /// </value>
    public Graphics Graphics
    {
        get
        {
            return graphics;
        }
    }

    /// <summary>
    /// The index of the item that should be painted.
    /// </summary>
    /// <value>
    /// The index.
    /// </value>
    public int Index { get; }

    /// <summary>
    /// Miscellaneous state information, such as whether the item is
    /// "selected", "focused", or some other such information.  ComboBoxes
    /// have one special piece of information which indicates if the item
    /// being painted is the editable portion of the ComboBox.
    /// </summary>
    /// <value>
    /// The state.
    /// </value>
    public DrawItemState State { get; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {

    }

    /// <summary>
    /// Fills the
    /// <see cref="T:System.Windows.Forms.DrawItemEventArgs.Bounds" /> with the
    /// <see cref="T:System.Windows.Forms.DrawItemEventArgs.BackColor" />.
    /// </summary>
    public virtual void DrawBackground()
    {
    }

    /// <summary>
    /// Draws a handy focus rect in the given rectangle.
    /// </summary>
    public virtual void DrawFocusRectangle()
    {
        if ((State & DrawItemState.Focus) == DrawItemState.Focus
            && (State & DrawItemState.NoFocusRect) != DrawItemState.NoFocusRect)
        {
        }
    }

    /// <summary>
    /// Returns the handle to a Windows device context.
    /// </summary>
    /// <returns>
    /// An System.IntPtr representing the handle of a device context.
    /// </returns>
    public IntPtr GetHdc()
    {
        return IntPtr.Zero;
    }

    /// <summary>
    /// Releases the handle of a Windows device context.
    /// </summary>
    public void ReleaseHdc()
    {

    }
}