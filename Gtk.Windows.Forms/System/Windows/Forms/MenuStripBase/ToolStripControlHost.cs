// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Hosts a <see cref="Control" /> inside a <see cref="ToolStrip" />. This is the public base class that
/// <see cref="ToolStripComboBox" />, <see cref="ToolStripTextBox" /> and <see cref="ToolStripProgressBar" />
/// conceptually derive from in WinForms, and the type a developer subclasses to put an arbitrary control on
/// a strip. The hosted control's GTK widget is embedded in the tool item, and the common item properties
/// (text, size, enabled, visible, font, colours) forward to it.
/// </summary>
public class ToolStripControlHost : ToolStripItem
{
    private readonly StripToolItem _self = new StripToolItem();
    private readonly Control _control;

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolStripControlHost" /> class hosting the specified
    /// control.
    /// </summary>
    /// <param name="c">The control to host. Cannot be <see langword="null" />.</param>
    /// <exception cref="ArgumentNullException"><paramref name="c" /> is <see langword="null" />.</exception>
    public ToolStripControlHost(Control c) : base()
    {
        _control = c ?? throw new ArgumentNullException(nameof(c));
        EmbedControl();
        OnSubscribeControlEvents(_control);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ToolStripControlHost" /> class hosting the specified
    /// control and giving the item the specified name.
    /// </summary>
    /// <param name="c">The control to host.</param>
    /// <param name="name">The name of the hosting item.</param>
    public ToolStripControlHost(Control c, string name) : this(c)
    {
        Name = name;
    }

    /// <summary>Gets the <see cref="Control" /> that this item is hosting.</summary>
    public Control Control => _control;

    /// <summary>Gets the GTK widget that represents this hosted item within the strip.</summary>
    public override IToolMenuItem Widget => _self;

    /// <summary>Gets or sets the text; forwards to the hosted control.</summary>
    public override string Text
    {
        get => _control != null ? _control.Text : base.Text;
        set
        {
            if (_control != null)
            {
                _control.Text = value;
            }
            else
            {
                base.Text = value;
            }
        }
    }

    /// <summary>Gets or sets the size; forwards to the hosted control.</summary>
    public override Size Size
    {
        get => base.Size;
        set
        {
            if (_control != null)
            {
                _control.Size = value;
            }

            base.Size = value;
        }
    }

    /// <summary>Gets or sets a value indicating whether the item is enabled; forwards to the hosted control.</summary>
    public override bool Enabled
    {
        get => base.Enabled;
        set
        {
            base.Enabled = value;
            if (_control != null)
            {
                _control.Enabled = value;
            }
        }
    }

    /// <summary>Gets or sets the font; forwards to the hosted control.</summary>
    public override Font Font
    {
        get => _control != null ? _control.Font : base.Font;
        set
        {
            if (_control != null)
            {
                _control.Font = value;
            }
            else
            {
                base.Font = value;
            }
        }
    }

    /// <summary>Gets or sets the background colour; forwards to the hosted control.</summary>
    public override Color BackColor
    {
        get => _control != null ? _control.BackColor : base.BackColor;
        set
        {
            if (_control != null)
            {
                _control.BackColor = value;
            }
            else
            {
                base.BackColor = value;
            }
        }
    }

    /// <summary>Gets or sets the foreground colour; forwards to the hosted control.</summary>
    public override Color ForeColor
    {
        get => _control != null ? _control.ForeColor : base.ForeColor;
        set
        {
            if (_control != null)
            {
                _control.ForeColor = value;
            }
            else
            {
                base.ForeColor = value;
            }
        }
    }

    /// <summary>Sets input focus to the hosted control.</summary>
    public void Focus() => _control?.Focus();

    /// <summary>Selects the hosted control.</summary>
    public void Select() => _control?.Select();

    /// <summary>
    /// Called when this item is hosting a control, to let a subclass subscribe to the control's events and
    /// re-raise them from the item. The base implementation does nothing.
    /// </summary>
    /// <param name="control">The hosted control.</param>
    protected virtual void OnSubscribeControlEvents(Control control)
    {
    }

    /// <summary>
    /// Called to let a subclass unsubscribe from the hosted control's events. The base implementation does
    /// nothing.
    /// </summary>
    /// <param name="control">The hosted control.</param>
    protected virtual void OnUnsubscribeControlEvents(Control control)
    {
    }

    /// <summary>Releases the resources used by the item, unsubscribing from the hosted control's events.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing && _control != null)
        {
            OnUnsubscribeControlEvents(_control);

            // Detach the hosted control's widget before the base disposes this item's widget. The control
            // still owns its widget, so letting the item destroy it would crash when the control is later
            // disposed.
            var widget = _control.Widget;
            if (widget != null && widget.Parent == _self)
            {
                _self.Remove(widget);
            }
        }

        base.Dispose(disposing);
    }

    private void EmbedControl()
    {
        var widget = _control?.Widget;
        if (widget == null)
        {
            return;
        }

        // Only parent the widget if it is not already owned; re-parenting an owned widget throws in GTK.
        if (widget.Parent == null)
        {
            _self.Add(widget);
            widget.ShowAll();
        }
    }
}
