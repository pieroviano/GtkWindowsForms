// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms;
using Gtk;

namespace System.Drawing.Printing;

/// <summary>
/// PrintDocument defines a reusable object that sends output to a printer. It is used in conjunction with the
/// <see cref="T:System.Windows.Forms.PrintDialog" /> class to display a dialog box that allows the user to select a printer and choose which sections of the document to print. The
/// <see cref="T:System.Drawing.Printing.PrintDocument" /> class raises events that you can handle to specify what to print, how to format the printed pages, and so on.
/// </summary>
/// <seealso cref="T:System.ComponentModel.Component" />
[DefaultProperty("DocumentName"), DefaultEvent("PrintPage")]
public partial class PrintDocument : Component
{

    private PrintEventHandler _beginPrintHandler = delegate { };

    private PageSettings _defaultPageSettings;

    private string _documentName = "document";

    private PrintEventHandler? _endPrintHandler;

    private bool _originAtMargins;

    private PageSetup _pageSetup;

    private PrinterSettings _printerSettings = new();

    private PrintPageEventHandler _printPageHandler = delegate { };

    private QueryPageSettingsEventHandler _queryHandler = delegate { };

    private bool _userSetPageSettings;

    /// <summary>
    /// Occurs when begin print.
    /// </summary>
    public virtual event PrintEventHandler BeginPrint
    {
        add => _beginPrintHandler += value;
#pragma warning disable CS8601 // Possible null reference assignment.
        remove => _beginPrintHandler -= value;
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    /// <summary>
    /// Occurs when end print.
    /// </summary>
    public virtual event PrintEventHandler EndPrint
    {
        add => _endPrintHandler += value;
        remove => _endPrintHandler -= value;
    }

    /// <summary>
    /// Occurs when print page.
    /// </summary>
    public virtual event PrintPageEventHandler PrintPage
    {
        add => _printPageHandler += value;
#pragma warning disable CS8601 // Possible null reference assignment.
        remove => _printPageHandler -= value;
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    /// <summary>
    /// Occurs when query page settings.
    /// </summary>
    public virtual event QueryPageSettingsEventHandler QueryPageSettings
    {
        add => _queryHandler += value;
#pragma warning disable CS8601 // Possible null reference assignment.
        remove => _queryHandler -= value;
#pragma warning restore CS8601 // Possible null reference assignment.
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Printing.PrintDocument" /> class.
    /// </summary>
    public PrintDocument()
    {
        _defaultPageSettings = new PageSettings(_printerSettings);
        _pageSetup = new PageSetup();
    }

    /// <summary>
    /// Gets or sets the default page settings.
    /// </summary>
    /// <value>
    /// The default page settings.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PageSettings DefaultPageSettings
    {
        get { return _defaultPageSettings; }
        set
        {
            _defaultPageSettings = value ?? new PageSettings();
            _userSetPageSettings = true;
        }
    }

    /// <summary>
    /// Gets or sets the name of the document.
    /// </summary>
    /// <value>
    /// The name of the document.
    /// </value>
    [DefaultValue("document")]
    public string DocumentName
    {
        get => _documentName;
        set => _documentName = value ?? "";
    }

    /// <summary>
    /// Gets or sets a value indicating whether origin at margins.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if origin at margins; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool OriginAtMargins
    {
        get => _originAtMargins;
        set => _originAtMargins = value;
    }

    /// <summary>
    /// Gets or sets the page setup.
    /// </summary>
    /// <value>
    /// The page setup.
    /// </value>
    public PageSetup PageSetup
    {
        get => _pageSetup;
        set
        {
            _pageSetup = value;
            var pageSettings = DefaultPageSettings;
            pageSettings.Landscape = value.Orientation == Gtk.PageOrientation.Landscape || value.Orientation == Gtk.PageOrientation.ReverseLandscape;
            // The margins were passed as (left, top, right, bottom) into a (left, right, top, bottom)
            // constructor - swapping top and right - and GTK points went in unconverted although
            // Margins and PaperSize are in hundredths of an inch.
            pageSettings.Margins = new Margins(
                PointsToHundredthsOfAnInch(value.GetLeftMargin(Unit.Points)),
                PointsToHundredthsOfAnInch(value.GetRightMargin(Unit.Points)),
                PointsToHundredthsOfAnInch(value.GetTopMargin(Unit.Points)),
                PointsToHundredthsOfAnInch(value.GetBottomMargin(Unit.Points)));
            if (!Enum.TryParse(value.PaperSize.DisplayName, out PaperKind paperKind))
            {
                paperKind = PaperKind.Custom;
            }

            pageSettings.PaperSize = new System.Drawing.Printing.PaperSize(
                paperKind,
                value.PaperSize.Name,
                PointsToHundredthsOfAnInch(value.PaperSize.GetWidth(Unit.Points)),
                PointsToHundredthsOfAnInch(value.PaperSize.GetHeight(Unit.Points)));
            _userSetPageSettings = true;
        }
    }

    /// <summary>
    /// Gets or sets the printer settings.
    /// </summary>
    /// <value>
    /// The printer settings.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PrinterSettings PrinterSettings
    {
        get => _printerSettings;
        set
        {
            value ??= new PrinterSettings();
            _printerSettings = value;

            // Reset the PageSettings that match the PrinterSettings only if we have created the defaultPageSettings.
            if (!_userSetPageSettings)
            {
                _defaultPageSettings = _printerSettings.DefaultPageSettings;
            }
        }
    }

    /// <summary>
    /// Prints this instance.
    /// </summary>
    public void Print()
    {
        var printDialog = new PrintDialog
        {
            Document = this,
            AllowPrintToFile = false,
            PrintToFile = false
        };
        printDialog.Document.DocumentName = DocumentName;
        // Print() used to show the print dialog. In WinForms it prints straight to the configured
        // printer; showing a dialog is the caller's choice via PrintDialog.
        printDialog.RunPrint(null, false);
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => $"[PrintDocument {DocumentName}]";

    private static int PointsToHundredthsOfAnInch(double points) => (int)Math.Round(points * 100.0 / 72.0);

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Drawing.Printing.PrintDocument.BeginPrint" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Drawing.Printing.PrintEventArgs" /> instance containing the event data.
    /// </param>
    protected internal virtual void OnBeginPrint(PrintEventArgs e) => _beginPrintHandler?.Invoke(this, e);

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Drawing.Printing.PrintDocument.EndPrint" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Drawing.Printing.PrintEventArgs" /> instance containing the event data.
    /// </param>
    protected internal virtual void OnEndPrint(PrintEventArgs e) => _endPrintHandler?.Invoke(this, e);

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Drawing.Printing.PrintDocument.PrintPage" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Drawing.Printing.PrintPageEventArgs" /> instance containing the event data.
    /// </param>
    protected internal virtual void OnPrintPage(PrintPageEventArgs e) => _printPageHandler?.Invoke(this, e);

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Drawing.Printing.PrintDocument.QueryPageSettings" /> event.
    /// </summary>
    /// <param name="e">
    /// The
    /// <see cref="T:System.Drawing.Printing.QueryPageSettingsEventArgs" /> instance containing the event data.
    /// </param>
    protected internal virtual void OnQueryPageSettings(QueryPageSettingsEventArgs e) => _queryHandler?.Invoke(this, e);

}