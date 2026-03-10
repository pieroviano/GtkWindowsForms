using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;

namespace System.Windows.Forms
{
    /// <summary>
    /// Lets users select a printer and choose which sections of the document to print from a Windows Forms application.
    /// </summary>
    [DefaultProperty(nameof(Document))]
    public sealed class PrintDialog : CommonDialog
    {
        /// <summary>
        /// The print document
        /// </summary>
        private PrintDocument _printDocument;

        /// <summary>
        /// The printer settings
        /// </summary>
        private PrinterSettings _printerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDialog"/> class.
        /// </summary>
        public PrintDialog() => Reset();

        /// <summary>
        /// Gets or sets a value indicating whether [allow current page].
        /// </summary>
        /// <value><c>true</c> if [allow current page]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]

        public bool AllowCurrentPage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow print to file].
        /// </summary>
        /// <value><c>true</c> if [allow print to file]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]

        public bool AllowPrintToFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow selection].
        /// </summary>
        /// <value><c>true</c> if [allow selection]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]

        public bool AllowSelection { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [allow some pages].
        /// </summary>
        /// <value><c>true</c> if [allow some pages]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]

        public bool AllowSomePages { get; set; }

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        /// <value>The document.</value>
        [DefaultValue(null)]

        public PrintDocument Document
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
        /// <value>The printer settings.</value>
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
        /// Gets or sets a value indicating whether [print to file].
        /// </summary>
        /// <value><c>true</c> if [print to file]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]

        public bool PrintToFile { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show help].
        /// </summary>
        /// <value><c>true</c> if [show help]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]

        public bool ShowHelp { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show network].
        /// </summary>
        /// <value><c>true</c> if [show network]; otherwise, <c>false</c>.</value>
        [DefaultValue(true)]

        public bool ShowNetwork { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use ex dialog].
        /// </summary>
        /// <value><c>true</c> if [use ex dialog]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]

        public bool UseEXDialog { get; set; }

        /// <summary>
        /// Gets the page settings.
        /// </summary>
        /// <value>The page settings.</value>
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
        /// <param name="owner">The owner.</param>
        /// <param name="showDialog">if set to <c>true</c> [show dialog].</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool RunPrint(IWin32Window owner, bool showDialog)
        {
            try
            {
                Gtk.PrintOperation printOperation = new Gtk.PrintOperation();
                printOperation.DefaultPageSetup = Document.PageSetup;
                printOperation.Unit = Unit.Points;
                printOperation.ShowProgress = true;
                printOperation.UseFullPage = true;
                printOperation.RequestPageSetup += PrintOperation_RequestPageSetup;
                printOperation.DrawPage += PrintOperation_DrawPage;
                printOperation.BeginPrint += PrintOperation_BeginPrint;
                printOperation.EndPrint += PrintOperation_EndPrint;
                printOperation.Preview += PrintOperation_Preview;
                Gtk.Window window = Gtk.Window.ListToplevels().LastOrDefault(o => o is FormBase && o.IsActive);
                if (PrintToFile && AllowPrintToFile)
                {
                    string exportFileName = _printerSettings.PrintFileName;
                    if (string.IsNullOrWhiteSpace(exportFileName))
                        exportFileName = $"print_{DateTime.Now.ToString("yyyyMMddHHmmss")}.pdf";
                    else if (System.IO.Path.GetExtension(exportFileName).ToLower() != ".pdf")
                        exportFileName += ".pdf";

                    printOperation.ExportFilename = exportFileName;
                    Gtk.PrintOperationResult result = printOperation.Run(PrintOperationAction.Export, owner == null ? window : ((Form)owner).self);
                    return result != Gtk.PrintOperationResult.Cancel && result != Gtk.PrintOperationResult.Error;
                }
                else
                {
                    Gtk.PrintOperationResult result = printOperation.Run(PrintOperationAction.PrintDialog, owner == null ? window : ((Form)owner).self);
                    return result != Gtk.PrintOperationResult.Cancel && result != Gtk.PrintOperationResult.Error;
                }
            }
            catch (Exception ex)
            {
                Gtk.MessageDialog messageDialog = new MessageDialog(owner == null ? null : ((Form)owner).self, DialogFlags.DestroyWithParent, MessageType.Error, ButtonsType.Ok, "");
                messageDialog.WindowPosition = owner == null ? WindowPosition.Center : WindowPosition.CenterOnParent;
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
        /// <param name="owner">The owner.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected override bool RunDialog(IWin32Window owner)
        {
            return RunPrint(owner, true);
        }

        /// <summary>
        /// Messages the dialog response.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The arguments.</param>
        private void MessageDialog_Response(object o, ResponseArgs args)
        {
            Gtk.MessageDialog messageDialog = (Gtk.MessageDialog)o;
            messageDialog.Destroy();
        }

        /// <summary>
        /// Prints the operation begin print.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The arguments.</param>
        private void PrintOperation_BeginPrint(object o, BeginPrintArgs args)
        {
            _printDocument?.OnBeginPrint(new PrintEventArgs());
            Gtk.PrintOperation oper = (Gtk.PrintOperation)o;
            oper.RenderPage(0);
        }

        /// <summary>
        /// Prints the operation draw page.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The arguments.</param>
        private void PrintOperation_DrawPage(object o, DrawPageArgs args)
        {
            Gtk.PrintOperation oper = (Gtk.PrintOperation)o;
            PageSetup setup = args.Context.PageSetup;
            using (var cr = args.Context.CairoContext)
            {
                //document size(794,1123)px
                double pxscale = 0.75;
                int top = (int)Math.Round(setup.GetTopMargin(oper.Unit) / pxscale, 0);
                int left = (int)Math.Round(setup.GetLeftMargin(oper.Unit) / pxscale, 0);
                int right = (int)Math.Round(setup.GetRightMargin(oper.Unit) / pxscale, 0);
                int bottom = (int)Math.Round(setup.GetBottomMargin(oper.Unit) / pxscale, 0);
                int width = (int)Math.Round(setup.GetPaperWidth(oper.Unit) / pxscale, 0); //page<paper
                int height = (int)Math.Round(setup.GetPaperHeight(oper.Unit) / pxscale, 0);

                cr.Scale(pxscale, pxscale);
                cr.Translate(0, 0);
                _printDocument?.OnPrintPage(new PrintPageEventArgs(new Drawing.Graphics(cr, new Gdk.Rectangle(0, 0, width, height)), new Drawing.Rectangle(left, top, width - left - right, height - top - bottom), new Drawing.Rectangle(0, 0, width, height), PageSettings));
            }
        }

        /// <summary>
        /// Prints the operation end print.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The arguments.</param>
        private void PrintOperation_EndPrint(object o, EndPrintArgs args)
        {
            _printDocument?.OnEndPrint(new PrintEventArgs());
        }

        /// <summary>
        /// Prints the operation preview.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The arguments.</param>
        private void PrintOperation_Preview(object o, PreviewArgs args)
        {
            args.Preview.RenderPage(0);
        }

        /// <summary>
        /// Prints the operation request page setup.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="args">The arguments.</param>
        private void PrintOperation_RequestPageSetup(object o, RequestPageSetupArgs args)
        {
            _printDocument?.OnQueryPageSettings(new QueryPageSettingsEventArgs(PageSettings));
        }

    }
}
