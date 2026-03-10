// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Gtk;
using System.ComponentModel;
using System.Windows.Forms;

namespace System.Drawing.Printing
{
    /// <summary>
    /// PrintDocument defines a reusable object that sends output to a printer. It is used in conjunction with the <see cref="PrintDialog"/> class to display a dialog box that allows the user to select a printer and choose which sections of the document to print. The <see cref="PrintDocument"/> class raises events that you can handle to specify what to print, how to format the printed pages, and so on.
    /// </summary>
    /// <seealso cref="System.ComponentModel.Component" />
    [DefaultProperty("DocumentName"), DefaultEvent("PrintPage")]
    public class PrintDocument : Component
    {
        private PrintEventHandler _beginPrintHandler;

        private PageSettings _defaultPageSettings;

        private string _documentName = "document";

        private PrintEventHandler _endPrintHandler;

        //private PrintController _printController;
        private bool _originAtMargins;

        private PageSetup _pageSetup;

        private PrinterSettings _printerSettings = new PrinterSettings();

        private PrintPageEventHandler _printPageHandler;

        private QueryPageSettingsEventHandler _queryHandler;

        private bool _userSetPageSettings;

        /// <summary>
        /// Occurs when begin print.
        /// </summary>
        public event PrintEventHandler BeginPrint { add => _beginPrintHandler += value; remove => _beginPrintHandler -= value; }

        /// <summary>
        /// Occurs when end print.
        /// </summary>
        public event PrintEventHandler EndPrint { add => _endPrintHandler += value; remove => _endPrintHandler -= value; }

        /// <summary>
        /// Occurs when print page.
        /// </summary>
        public event PrintPageEventHandler PrintPage { add => _printPageHandler += value; remove => _printPageHandler -= value; }

        /// <summary>
        /// Occurs when query page settings.
        /// </summary>
        public event QueryPageSettingsEventHandler QueryPageSettings { add => _queryHandler += value; remove => _queryHandler -= value; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrintDocument"/> class.
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
            get
            {
                return _defaultPageSettings;
            }

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
        public string DocumentName { get => _documentName; set => _documentName = value ?? ""; }

        /// <summary>
        /// Gets or sets a value indicating whether [origin at margins].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [origin at margins]; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(false)]
        public bool OriginAtMargins { get => _originAtMargins; set => _originAtMargins = value; }

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
                PageSettings pageSettings = DefaultPageSettings;
                pageSettings.Landscape = value.Orientation == Gtk.PageOrientation.Landscape || value.Orientation == Gtk.PageOrientation.ReverseLandscape;
                pageSettings.Margins = new Margins((int)value.GetLeftMargin(Unit.Points), (int)value.GetTopMargin(Unit.Points), (int)value.GetRightMargin(Unit.Points), (int)value.GetBottomMargin(Unit.Points));
                Enum.TryParse(value.PaperSize.DisplayName, out PaperKind paperKind);
                pageSettings.PaperSize = new System.Drawing.Printing.PaperSize(paperKind, value.PaperSize.Name, (int)value.PaperSize.GetWidth(Unit.Points), (int)value.PaperSize.GetHeight(Unit.Points));
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
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = this;
            printDialog.AllowPrintToFile = false;
            printDialog.PrintToFile = false;
            printDialog.Document.DocumentName = "PrintDoument";
            printDialog.RunPrint(null, true);
        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => $"[PrintDocument {DocumentName}]";

        /// <summary>
        /// Raises the <see cref="E:BeginPrint" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PrintEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnBeginPrint(PrintEventArgs e) => _beginPrintHandler?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="E:EndPrint" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PrintEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnEndPrint(PrintEventArgs e) => _endPrintHandler?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="E:PrintPage" /> event.
        /// </summary>
        /// <param name="e">The <see cref="PrintPageEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnPrintPage(PrintPageEventArgs e) => _printPageHandler?.Invoke(this, e);

        /// <summary>
        /// Raises the <see cref="E:QueryPageSettings" /> event.
        /// </summary>
        /// <param name="e">The <see cref="QueryPageSettingsEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnQueryPageSettings(QueryPageSettingsEventArgs e) => _queryHandler?.Invoke(this, e);

    }
}
