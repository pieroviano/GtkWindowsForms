// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a dialog box form that contains a
/// <see cref="T:System.Windows.Forms.PrintPreviewControl" />
/// for printing from a Windows Forms application.
/// </summary>
[DesignTimeVisible(true)]
//[DefaultProperty(nameof(Document))]
[ToolboxItemFilter("System.Windows.Forms.Control.TopLevel")]
[ToolboxItem(true)]
public partial class PrintPreviewDialog : ScrollableControl
{
    /// <summary>
    /// The self
    /// </summary>
    public ViewportBase self = new ViewportBase();

    private PrintPreviewControl _previewControl;

    private Gtk.Box box;

    private Form previewForm;

    /// <summary>
    /// Occurs when [maximum size changed.
    /// </summary>
    public event EventHandler MaximumSizeChanged;

    /// <summary>
    /// Occurs when [minimum size changed.
    /// </summary>
    public event EventHandler MinimumSizeChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PrintPreviewDialog" /> class.
    /// </summary>
    public PrintPreviewDialog() : base()
    {

    }

    /// <summary>
    /// Gets or sets the accept button.
    /// </summary>
    /// <value>
    /// The accept button.
    /// </value>
    public IButtonControl AcceptButton
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic scale.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if automatic scale; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool AutoScale
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the cancel button.
    /// </summary>
    /// <value>
    /// The cancel button.
    /// </value>
    public IButtonControl CancelButton
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether control box.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if control box; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool ControlBox
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    /// <value>
    /// The document.
    /// </value>
    public PrintDocument Document
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl { get => self; }

    /// <summary>
    /// Gets or sets a value indicating whether help button.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if help button; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool HelpButton
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the icon.
    /// </summary>
    /// <value>
    /// The icon.
    /// </value>
    public System.Drawing.Icon Icon
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is MDI container.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if this instance is MDI container; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool IsMdiContainer
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether key preview.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if key preview; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool KeyPreview
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the print preview control.
    /// </summary>
    /// <value>
    /// The print preview control.
    /// </value>
    public PrintPreviewControl PrintPreviewControl => _previewControl;

    /// <summary>
    /// Gets or sets the size grip style.
    /// </summary>
    /// <value>
    /// The size grip style.
    /// </value>
    public SizeGripStyle SizeGripStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the start position.
    /// </summary>
    /// <value>
    /// The start position.
    /// </value>
    public FormStartPosition StartPosition
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether top most.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if top most; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool TopMost
    {
        get;
        set;
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
    public bool UseAntiAlias
    {
        get => PrintPreviewControl.UseAntiAlias;
        set => PrintPreviewControl.UseAntiAlias = value;
    }

    /// <summary>
    /// Shows the control.
    /// </summary>
    public override void Show()
    {
        this.Show(null);
    }

    /// <summary>
    /// Shows the specified owner.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <exception cref="T:System.InvalidOperationException">
    /// OwnsSelfOrOwner
    /// </exception>
    public void Show(IWin32Window owner)
    {
        if (owner == this)
        {
            throw new InvalidOperationException("OwnsSelfOrOwner");
        }
        Init();
        previewForm.Show(owner);
    }

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <returns>
    /// DialogResult.
    /// </returns>
    public DialogResult ShowDialog(IWin32Window owner)
    {
        Init();

        return previewForm.ShowDialog(owner);
    }

    /// <summary>
    /// Shows the dialog.
    /// </summary>
    /// <returns>
    /// DialogResult.
    /// </returns>
    public DialogResult ShowDialog()
    {
        return ShowDialog(null);
    }

    private void Init()
    {
        previewForm = new Form
        {
            AutoScroll = true
        };
        previewForm.self.Resizable = true;
        _previewControl = new PrintPreviewControl();
        //document size(794,1123)px
        _previewControl.self.MarginBottom = 20;
        _previewControl.Zoom = 1;
        _previewControl.AutoZoom = true;
        _previewControl.Document = Document;
        int formwidth = _previewControl.Width + 100;
        int formheight = _previewControl.Height + 100;
        previewForm.ClientSize = new Size(formwidth, formheight);
        box = new Gtk.Box(Gtk.Orientation.Vertical, 15);
        var header = new Gtk.Box(Gtk.Orientation.Horizontal, 20);
        header.PackStart(new Gtk.Label(System.Properties.Resources.Gtk_Print_preview), false, false, 0);
        var printbutton = new Gtk.Button(System.Properties.Resources.Gtk_Print) { WidthRequest = 200 };
        printbutton.ButtonReleaseEvent += Printbutton_ButtonReleaseEvent;
        header.PackEnd(printbutton, false, false, 0);
        box.PackStart(header, false, true, 0);
        box.PackStart(_previewControl.Widget, false, true, 0);
        box.MarginTop = 20;
        box.MarginStart = (previewForm.Width - _previewControl.Width) / 2;
        previewForm.Controls.Add(box);
        previewForm.SizeChanged += PreviewForm_SizeChanged;
    }

    private void PreviewForm_SizeChanged(object sender, EventArgs e)
    {
        box.MarginStart = (previewForm.Width - _previewControl.Width) / 2;
    }

    private void Printbutton_ButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
    {
        var printDialog = new PrintDialog
        {
            Document = _previewControl.Document,
            AllowPrintToFile = false,
            PrintToFile = false
        };
        printDialog.Document.DocumentName = "PrintDoument";
        printDialog.ShowDialog(previewForm);
    }

    private void Printbutton_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
    {
        var printDialog = new PrintDialog
        {
            Document = _previewControl.Document,
            AllowPrintToFile = false,
            PrintToFile = false
        };
        printDialog.Document.DocumentName = "PrintDoument";
        printDialog.ShowDialog(previewForm);
    }

}