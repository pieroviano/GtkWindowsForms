// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents the raw preview part of print previewing from a Windows Forms application, without any dialog boxes or buttons. Most
/// <see cref="T:System.Windows.Forms.PrintPreviewControl" /> objects are found on
/// <see cref="T:System.Windows.Forms.PrintPreviewDialog" />
/// objects, but they do not have to be.
/// </summary>
[DefaultProperty(nameof(Document))]
public partial class PrintPreviewControl : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly ViewportBase self = new();

    private bool _autoZoom = true;

    private int _columns = 1;

    private PrintDocument? _document;

    private int _rows = 1;

    private int _startPage;  // 0-based

    private double _zoom = DefaultZoom;

    // Spacing per page, in mm

    private const int Border = 10;

    private const double DefaultZoom = .3;

    private bool Is_Self_Realized;

    private readonly ViewportBase paper;

    private const double pxscale = 0.75;

    private static readonly object s_startPageChangedEvent = new();

    private readonly Gtk.ScrolledWindow scroll;

    private const int ScrollSmallChange = 5;

    /// <summary>
    /// Occurs when start page changed.
    /// </summary>
    public virtual event EventHandler StartPageChanged
    {
        add => Events.AddHandler(s_startPageChangedEvent, value);
        remove => Events.RemoveHandler(s_startPageChangedEvent, value);
    }

    /// <summary>
    /// Occurs when text changed.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PrintPreviewControl" /> class.
    /// </summary>
    public PrintPreviewControl()
    {
        Size = new Size(600, 500);
        self.StyleContext.AddClass("PrintPreviewBack");

        paper = new ViewportBase
        {
            Valign = Align.Center,
            Halign = Align.Center
        };
        paper.StyleContext.AddClass("Paper");
        // Gtk 4: Widget.Drawn is gone and there is no overridable OnSnapshot; custom paint moves to CSS (deferred).
        scroll = new Gtk.ScrolledWindow
        {
            Child = paper
        };
        self.AddChild(scroll);
        self.Realized += Self_Realized;
        ResetBackColor();
        ResetForeColor();
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic zoom.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic zoom; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool AutoZoom
    {
        get => _autoZoom;
        set
        {
            if (_autoZoom != value)
            {
                _autoZoom = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    /// <value>
    /// The columns.
    /// </value>
    [DefaultValue(1)]
    public int Columns
    {
        get => _columns;
        set
        {
            _columns = value;
        }
    }

    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    /// <value>
    /// The document.
    /// </value>
    [DefaultValue(null)]
    public PrintDocument? Document
    {
        get => _document;
        set
        {
            _document = value;
            paper.WidthRequest = (int)Math.Round((_document?.PageSetup.GetPaperWidth(Unit.Points) ?? 0) / pxscale, 0);
            paper.HeightRequest = (int)Math.Round((_document?.PageSetup.GetPaperHeight(Unit.Points) ?? 0) / pxscale, 0);
            self.WidthRequest = paper.WidthRequest + 100;
            self.HeightRequest = paper.HeightRequest + 100;
            InvalidatePreview();
        }
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the right to left.
    /// </summary>
    /// <value>
    /// The right to left.
    /// </value>
    [Localizable(true)]
    [AmbientValue(RightToLeft.Inherit)]
    public override RightToLeft RightToLeft
    {
        get => base.RightToLeft;
        set
        {
            base.RightToLeft = value;
            InvalidatePreview();
        }
    }

    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    /// <value>
    /// The rows.
    /// </value>
    [DefaultValue(1)]
    public int Rows
    {
        get => _rows;
        set
        {
            _rows = value;
        }
    }

    /// <summary>
    /// Gets or sets the start page.
    /// </summary>
    /// <value>
    /// The start page.
    /// </value>
    [DefaultValue(0)]
    public int StartPage
    {
        get
        {
            var value = _startPage;
            value = Math.Max(value, 0);

            return value;
        }
        set
        {
            var oldValue = StartPage;
            _startPage = value;
            if (oldValue != _startPage)
            {
                OnStartPageChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether tab stop.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if tab stop; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether use anti alias.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use anti alias; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool UseAntiAlias
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the zoom.
    /// </summary>
    /// <value>
    /// The zoom.
    /// </value>
    [DefaultValue(DefaultZoom)]
    public double Zoom
    {
        get => _zoom;
        set
        {
            _autoZoom = false;
            _zoom = value;
        }
    }

    /// <summary>
    /// Invalidates the preview.
    /// </summary>
    public void InvalidatePreview()
    {
        paper.QueueDraw();
    }

    /// <summary>
    /// Resets the BackColor property to its default value.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ResetBackColor() => BackColor = SystemColors.AppWorkspace;

    /// <summary>
    /// Resets the ForeColor property to its default value.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override void ResetForeColor() => ForeColor = Color.White;

    /// <summary>
    /// Handles the
    /// <see cref="T:System.Windows.Forms.PrintPreviewControl.StartPageChanged" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.EventArgs" /> instance containing the event data.
    /// </param>
    protected virtual void OnStartPageChanged(EventArgs e)
    {
        ((EventHandler)Events[s_startPageChangedEvent])?.Invoke(this, e);
    }

    private void Self_Realized(object? sender, EventArgs e)
    {
        if (!Is_Self_Realized)
        {
            Is_Self_Realized = true;
            if (AutoZoom == false)
            {
                paper.WidthRequest = (int)Math.Round((_document?.PageSetup.GetPaperWidth(Unit.Points) ?? 0) * _zoom / 0.75, 0);
                paper.HeightRequest = (int)Math.Round((_document?.PageSetup.GetPaperHeight(Unit.Points) ?? 0) * _zoom / 0.75, 0);
            }
        }
    }
}