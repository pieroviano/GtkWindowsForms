using System.ComponentModel;
using System.Drawing.Printing;
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Lets users select a printer and choose which sections of the document to print from a Windows Forms application.
/// </summary>
[DefaultProperty(nameof(Document))]
public sealed class PrintDialog : CommonDialog
{
    private PrintDocument? _printDocument;

    private PrinterSettings? _printerSettings;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.PrintDialog" /> class.
    /// </summary>
    public PrintDialog() => Reset();

    /// <summary>
    /// Gets or sets a value indicating whether allow current page.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow current page; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool AllowCurrentPage
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow print to file.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow print to file; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(true)]
    public bool AllowPrintToFile
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow selection.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow selection; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool AllowSelection
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether allow some pages.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if allow some pages; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool AllowSomePages
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
    public PrintDocument? Document
    {
        get => _printDocument;
        set
        {
            _printDocument = value;
            _printerSettings = _printDocument is null ? new PrinterSettings() : _printDocument.PrinterSettings;
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
        get => _printerSettings ??= new PrinterSettings();
        set
        {
            if (value != PrinterSettings)
            {
                _printerSettings = value;
                _printDocument = null;
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether print to file.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if print to file; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool PrintToFile
    {
        get;
        set;
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
    /// Gets or sets a value indicating if we use ex dialog.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if we use ex dialog; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    [DefaultValue(false)]
    public bool UseEXDialog
    {
        get;
        set;
    }

    private PageSettings PageSettings => Document is null
        ? PrinterSettings.DefaultPageSettings
        : Document.DefaultPageSettings;

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public override void Reset()
    {
        AllowCurrentPage = false;
        AllowSomePages = false;
        AllowPrintToFile = true;
        AllowSelection = false;
        _printDocument = null;
        PrintToFile = false;
        _printerSettings = null;
        ShowHelp = false;
        ShowNetwork = true;
    }

    /// <summary>
    /// Runs the print.
    /// </summary>
    /// <param name="owner">
    /// The owner.
    /// </param>
    /// <param name="showDialog">
    /// if set to
    /// <c>
    /// true
    /// </c>show dialog.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if XXXX,
    /// <c>
    /// false
    /// </c> otherwise.
    /// </returns>
    public bool RunPrint(IWin32Window? owner, bool showDialog)
    {
        try
        {
            var printOperation = new Gtk.PrintOperation
            {
                DefaultPageSetup = Document?.PageSetup,
                Unit = Unit.Points,
                ShowProgress = true,
                UseFullPage = true
            };
            printOperation.RequestPageSetup += PrintOperation_RequestPageSetup;
            printOperation.DrawPage += PrintOperation_DrawPage;
            printOperation.BeginPrint += PrintOperation_BeginPrint;
            printOperation.EndPrint += PrintOperation_EndPrint;
            printOperation.Preview += PrintOperation_Preview;
            var window = Gtk.Window.ListToplevels().LastOrDefault(o => o is FormBase && o.IsActive);
            if (PrintToFile && AllowPrintToFile)
            {
                var exportFileName = _printerSettings?.PrintFileName;
                if (string.IsNullOrWhiteSpace(exportFileName))
                    exportFileName = $"print_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
                else if (System.IO.Path.GetExtension(exportFileName).ToLower() != ".pdf")
                    exportFileName += ".pdf";

                printOperation.ExportFilename = exportFileName;
                var result = printOperation.Run(PrintOperationAction.Export, owner == null ? window : ((Form)owner).self);
                return result != Gtk.PrintOperationResult.Cancel && result != Gtk.PrintOperationResult.Error;
            }
            else
            {
                var result = printOperation.Run(PrintOperationAction.PrintDialog, owner == null ? window : ((Form)owner).self);
                return result != Gtk.PrintOperationResult.Cancel && result != Gtk.PrintOperationResult.Error;
            }
        }
        catch (Exception ex)
        {
            var messageDialog = new MessageDialog(owner == null ? null : ((Form)owner).self, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok, "")
            {
                WindowPosition = owner == null ? WindowPosition.Center : WindowPosition.CenterOnParent
            };
            if (ex.Message.ToLower().Contains("doc"))
                messageDialog.Text = System.Properties.Resources.Gtk_PrintDialog_RunPrint_File_is_in_use_and_cannot_be_overwritten;
            else
                messageDialog.Text = ex.Message;
            messageDialog.Response += MessageDialog_Response;
            messageDialog.ShowAll();
            return false;
        }
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
    protected override bool RunDialog(IWin32Window? owner)
    {
        return RunPrint(owner, true);
    }

    private void MessageDialog_Response(object o, ResponseArgs args)
    {
        var messageDialog = (Gtk.MessageDialog)o;
        messageDialog.Destroy();
    }

    private void PrintOperation_BeginPrint(object o, BeginPrintArgs args)
    {
        _printDocument?.OnBeginPrint(new PrintEventArgs());
        var oper = (Gtk.PrintOperation)o;
        oper.RenderPage(0);
    }

    private void PrintOperation_DrawPage(object o, DrawPageArgs args)
    {
        var oper = (Gtk.PrintOperation)o;
        var setup = args.Context.PageSetup;
        using (var cr = args.Context.CairoContext)
        {
            //document size(794,1123)px
            var pxscale = 0.75;
            var top = (int)Math.Round(setup.GetTopMargin(oper.Unit) / pxscale, 0);
            var left = (int)Math.Round(setup.GetLeftMargin(oper.Unit) / pxscale, 0);
            var right = (int)Math.Round(setup.GetRightMargin(oper.Unit) / pxscale, 0);
            var bottom = (int)Math.Round(setup.GetBottomMargin(oper.Unit) / pxscale, 0);
            var width = (int)Math.Round(setup.GetPaperWidth(oper.Unit) / pxscale, 0); //page<paper
            var height = (int)Math.Round(setup.GetPaperHeight(oper.Unit) / pxscale, 0);

            cr.Scale(pxscale, pxscale);
            cr.Translate(0, 0);
            _printDocument?.OnPrintPage(new PrintPageEventArgs(new Drawing.Graphics(cr, new Gdk.Rectangle(0, 0, width, height)), new Drawing.Rectangle(left, top, width - left - right, height - top - bottom), new Drawing.Rectangle(0, 0, width, height), PageSettings));
        }
    }

    private void PrintOperation_EndPrint(object o, EndPrintArgs args)
    {
        _printDocument?.OnEndPrint(new PrintEventArgs());
    }

    private void PrintOperation_Preview(object o, PreviewArgs args)
    {
        args.Preview.RenderPage(0);
    }

    private void PrintOperation_RequestPageSetup(object o, RequestPageSetupArgs args)
    {
        _printDocument?.OnQueryPageSettings(new QueryPageSettingsEventArgs(PageSettings));
    }
}