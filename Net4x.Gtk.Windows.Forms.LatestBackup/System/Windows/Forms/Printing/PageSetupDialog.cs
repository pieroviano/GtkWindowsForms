// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Printing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Enables users to change page-related print settings, including margins and paper orientation. This class cannot be inherited.
/// </summary>
[DefaultProperty(nameof(Document))]
public sealed class PageSetupDialog : CommonDialog
{

    private Margins _minMargins;

    private PageSettings _pageSettings;

    private PrintDocument _printDocument;

    private PrinterSettings _printerSettings;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PageSetupDialog" /> class.
    /// </summary>
    public PageSetupDialog() => Reset();

    /// <summary>
    /// Gets or sets a value indicating whether allow margins.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow margins; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool AllowMargins
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow orientation.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow orientation; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool AllowOrientation
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow paper.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow paper; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool AllowPaper
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow printer.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow printer; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool AllowPrinter
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
    [DefaultValue(null)]
    public PrintDocument Document
    {
        get => _printDocument;

        set
        {
            _printDocument = value;
            if (_printDocument != null)
            {
                _pageSettings = _printDocument.DefaultPageSettings;
                _printerSettings = _printDocument.PrinterSettings;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether enable metric.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if enable metric; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public bool EnableMetric
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the minimum margins.
    /// </summary>
    /// <value>
    /// The minimum margins.
    /// </value>
    public Margins MinMargins
    {
        get => _minMargins;

        set => _minMargins = value ?? new Margins(0, 0, 0, 0);
    }

    /// <summary>
    /// Gets or sets the page settings.
    /// </summary>
    /// <value>
    /// The page settings.
    /// </value>
    [DefaultValue(null)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PageSettings PageSettings
    {
        get => _pageSettings;

        set
        {
            _pageSettings = value;
            _printDocument = null;
        }
    }

    /// <summary>
    /// Gets or sets the printer settings.
    /// </summary>
    /// <value>
    /// The printer settings.
    /// </value>
    [DefaultValue(null)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PrinterSettings PrinterSettings
    {
        get => _printerSettings;

        set
        {
            _printerSettings = value;
            _printDocument = null;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether show help.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show help; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool ShowHelp
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether show network.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if show network; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool ShowNetwork
    {
        get;
        set;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public override void Reset()
    {
        AllowMargins = true;
        AllowOrientation = true;
        AllowPaper = true;
        AllowPrinter = true;
        MinMargins = null; // turns into Margin with all zeros
        _pageSettings = null;
        _printDocument = null;
        _printerSettings = null;
        ShowHelp = false;
        ShowNetwork = true;
    }

    /// <summary>
    /// Runs the dialog.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    protected override bool RunDialog(IWin32Window owner)
    {
        try
        {
            Gtk.Window window = Gtk.Window.ListToplevels().LastOrDefault(o => o is FormBase && o.IsActive);
            var printSettings = new Gtk.PrintSettings();
            PageSetup pageSetup = Gtk.Print.RunPageSetupDialog(owner == null ? window : ((Form)owner).self, _printDocument.PageSetup ?? new Gtk.PageSetup(), printSettings);
            _printDocument.PageSetup = pageSetup;
            return true;
        }
        catch
        {
            return false;
        }
    }

}