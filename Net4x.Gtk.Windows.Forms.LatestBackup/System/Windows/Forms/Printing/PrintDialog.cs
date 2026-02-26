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
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DefaultProperty(nameof(Document))]
    public sealed class PrintDialog : CommonDialog
    {

        private PrinterSettings _printerSettings;
        private PrintDocument _printDocument;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PrintDialog" /> class.
        /// </summary>
        public PrintDialog() => Reset();
        /// <summary>
        /// Gets or sets a value indicating whether the Current Page option button is displayed.
        /// </summary>
        /// <returns>true if the Current Page option button is displayed; otherwise, false. The default is false.
        /// </returns>
        [DefaultValue(false)]
        public bool AllowCurrentPage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Pages option button is enabled.
        /// </summary>
        /// <returns>true if the Pages option button is enabled; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public bool AllowSomePages { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Print to file check box is enabled.
        /// </summary>
        /// <returns>true if the Print to file check box is enabled; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public bool AllowPrintToFile { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Selection option button is enabled.
        /// </summary>
        /// <returns>true if the Selection option button is enabled; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public bool AllowSelection { get; set; }
        /// <summary>
        /// Gets or sets a value indicating the
        /// <see cref="System.Drawing.Printing.PrintDocument" /> used to obtain
        /// <see cref="System.Drawing.Printing.PrinterSettings" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Printing.PrintDocument" /> used to obtain
        /// <see cref="System.Drawing.Printing.PrinterSettings" /> The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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

        private PageSettings PageSettings => Document is null
            ? PrinterSettings.DefaultPageSettings
            : Document.DefaultPageSettings;
        /// <summary>
        /// Gets or sets the printer settings the dialog box modifies.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Printing.PrinterSettings" /> the dialog box modifies.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// Gets or sets a value indicating whether the Print to file check box is selected.
        /// </summary>
        /// <returns>true if the Print to file check box is selected; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public bool PrintToFile { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Help button is displayed.
        /// </summary>
        /// <returns>true if the Help button is displayed; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public bool ShowHelp { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Network button is displayed.
        /// </summary>
        /// <returns>true if the Network button is displayed; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public bool ShowNetwork { get; set; }                                                                                        /// <summary>
                                                                                                                                     /// Gets or sets a value indicating whether the dialog should be shown in the Windows XP style for systems running Windows XP Home Edition, Windows XP Professional, Windows Server 2003 or later.
                                                                                                                                     /// </summary>
                                                                                                                                     /// <returns>true to indicate the dialog should be shown with the Windows XP style, otherwise false. The default is false.
                                                                                                                                     /// </returns>
        [DefaultValue(false)]
        public bool UseEXDialog { get; set; }
        /// <summary>
        /// Resets all options, the last selected printer, and the page settings to their default values.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
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

        protected override bool RunDialog(IWin32Window owner)
        {
            return RunPrint(owner, true);
        }
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

        private void PrintOperation_Preview(object o, PreviewArgs args)
        {
            args.Preview.RenderPage(0);
        }

        private void MessageDialog_Response(object o, ResponseArgs args)
        {
            Gtk.MessageDialog messageDialog = (Gtk.MessageDialog)o;
            messageDialog.Destroy();
        }

        private void PrintOperation_RequestPageSetup(object o, RequestPageSetupArgs args)
        {
            _printDocument?.OnQueryPageSettings(new QueryPageSettingsEventArgs(PageSettings));
        }

        private void PrintOperation_EndPrint(object o, EndPrintArgs args)
        {
            _printDocument?.OnEndPrint(new PrintEventArgs());
        }

        private void PrintOperation_BeginPrint(object o, BeginPrintArgs args)
        {
            _printDocument?.OnBeginPrint(new PrintEventArgs());
            Gtk.PrintOperation oper = (Gtk.PrintOperation)o;
            oper.RenderPage(0);
        }

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
    }
}
