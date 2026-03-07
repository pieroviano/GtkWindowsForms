// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// This event is fired by owner drawn
    /// <see cref="System.Windows.Forms.Control" />
    /// objects, such as
    /// <see cref="System.Windows.Forms.ListBox" /> and
    /// <see cref="System.Windows.Forms.ComboBox" />
    /// It contains all the information needed for the user to paint the given item,
    /// including the item index, the
    /// <see cref="System.Drawing.Rectangle" />
    /// in which the drawing should be done, and the
    /// <see cref="System.Windows.Forms.DrawItemEventArgs.Graphics" />
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

        private Graphics graphics;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DrawItemEventArgs" /> class for the specified control with the specified font, state, surface to draw on, and the bounds to draw within.
        /// </summary>
        /// <param name="graphics">The
        /// <see cref="System.Drawing.Graphics" /> surface on which to draw.
        /// </param>
        /// <param name="font">The
        /// <see cref="System.Drawing.Font" /> to use, usually the parent control's
        /// <see cref="System.Drawing.Font" /> property.
        /// </param>
        /// <param name="rect">The
        /// <see cref="System.Drawing.Rectangle" /> bounds to draw within.
        /// </param>
        /// <param name="index">The
        /// <see cref="System.Windows.Forms.Control.ControlCollection" /> index value of the item that is being drawn.
        /// </param>
        /// <param name="state">The control's
        /// <see cref="System.Windows.Forms.DrawItemState" /> information.
        /// </param>
        public DrawItemEventArgs(Graphics graphics, Font? font, Rectangle rect, int index, DrawItemState state)
            : this(graphics, font, rect, index, state, SystemColors.WindowText, SystemColors.Window)
        { }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DrawItemEventArgs" /> class for the specified control with the specified font, state, foreground color, background color, surface to draw on, and the bounds to draw within.
        /// </summary>
        /// <param name="graphics">The
        /// <see cref="System.Drawing.Graphics" /> surface on which to draw.
        /// </param>
        /// <param name="font">The
        /// <see cref="System.Drawing.Font" /> to use, usually the parent control's
        /// <see cref="System.Drawing.Font" /> property.
        /// </param>
        /// <param name="rect">The
        /// <see cref="System.Drawing.Rectangle" /> bounds to draw within.
        /// </param>
        /// <param name="index">The
        /// <see cref="System.Windows.Forms.Control.ControlCollection" /> index value of the item that is being drawn.
        /// </param>
        /// <param name="state">The control's
        /// <see cref="System.Windows.Forms.DrawItemState" /> information.
        /// </param>
        /// <param name="foreColor">The foreground
        /// <see cref="System.Drawing.Color" /> to draw the control with.
        /// </param>
        /// <param name="backColor">The background
        /// <see cref="System.Drawing.Color" /> to draw the control with.
        /// </param>
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
        /// Gets the background color of the item that is being drawn.
        /// </summary>
        /// <returns>The background
        /// <see cref="System.Drawing.Color" /> of the item that is being drawn.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color BackColor
                    => (State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.Highlight : _backColor;

        /// <summary>
        /// Gets the rectangle that represents the bounds of the item that is being drawn.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Rectangle" /> that represents the bounds of the item that is being drawn.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Rectangle Bounds { get; }

        /// <summary>
        /// Gets the font that is assigned to the item being drawn.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Font" /> that is assigned to the item being drawn.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Font? Font { get; }

        /// <summary>
        /// Gets the foreground color of the of the item being drawn.
        /// </summary>
        /// <returns>The foreground
        /// <see cref="System.Drawing.Color" /> of the item being drawn.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Color ForeColor
            => (State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : _foreColor;

        /// <summary>
        /// Gets the graphics surface to draw the item on.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Graphics" /> surface to draw the item on.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Graphics Graphics
        {
            get
            {
                return graphics;
            }
        }

        /// <summary>
        /// Gets the index value of the item that is being drawn.
        /// </summary>
        /// <returns>The numeric value that represents the
        /// <see cref="System.Windows.Forms.Control.ControlCollection.Item(System.Int32)" /> value of the item being drawn.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Index { get; }

        /// <summary>
        /// Miscellaneous state information, such as whether the item is
        /// "selected", "focused", or some other such information.  ComboBoxes
        /// have one special piece of information which indicates if the item
        /// being painted is the editable portion of the ComboBox.
        /// </summary>
        public DrawItemState State { get; }

        public void Dispose()
        {

        }

        /// <summary>
        /// Draws the background within the bounds specified in the
        /// <see cref="!:Overload:System.Windows.Forms.DrawItemEventArgs.#ctor" /> constructor and with the appropriate color.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void DrawBackground()
        {
        }

        /// <summary>
        /// Draws a focus rectangle within the bounds specified in the
        /// <see cref="!:Overload:System.Windows.Forms.DrawItemEventArgs.#ctor" /> constructor.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual void DrawFocusRectangle()
        {
            if ((State & DrawItemState.Focus) == DrawItemState.Focus
                && (State & DrawItemState.NoFocusRect) != DrawItemState.NoFocusRect)
            {
            }
        }

        public IntPtr GetHdc()
        {
            return IntPtr.Zero;
        }

        public void ReleaseHdc()
        {

        }

    }
}
