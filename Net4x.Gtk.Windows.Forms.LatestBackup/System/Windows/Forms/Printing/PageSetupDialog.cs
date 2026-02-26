// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;

namespace System.Windows.Forms
{
    /// <summary>
    /// Enables users to change page-related print settings, including margins and paper orientation. This class cannot be inherited.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DefaultProperty(nameof(Document))]
    public sealed class PageSetupDialog : CommonDialog
    {

        private PrintDocument _printDocument;

        private PageSettings _pageSettings;
        private PrinterSettings _printerSettings;

        private Margins _minMargins;
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PageSetupDialog" /> class.
        /// </summary>
        public PageSetupDialog() => Reset();
        /// <summary>
        /// Gets or sets a value indicating whether the margins section of the dialog box is enabled.
        /// </summary>
        /// <returns>true if the margins section of the dialog box is enabled; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public bool AllowMargins { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the orientation section of the dialog box (landscape versus portrait) is enabled.
        /// </summary>
        /// <returns>true if the orientation section of the dialog box is enabled; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public bool AllowOrientation { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the paper section of the dialog box (paper size and paper source) is enabled.
        /// </summary>
        /// <returns>true if the paper section of the dialog box is enabled; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public bool AllowPaper { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Printer button is enabled.
        /// </summary>
        /// <returns>true if the Printer button is enabled; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public bool AllowPrinter { get; set; }
        /// <summary>
        /// Gets or sets a value indicating the
        /// <see cref="System.Drawing.Printing.PrintDocument" /> to get page settings from.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Printing.PrintDocument" /> to get page settings from. The default is null.
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
                if (_printDocument != null)
                {
                    _pageSettings = _printDocument.DefaultPageSettings;
                    _printerSettings = _printDocument.PrinterSettings;
                }
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether the margin settings, when displayed in millimeters, should be automatically converted to and from hundredths of an inch.
        /// </summary>
        /// <returns>true if the margins should be automatically converted; otherwise, false. The default is false.
        /// </returns>
        [DefaultValue(false)]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public bool EnableMetric { get; set; }
        /// <summary>
        /// Gets or sets a value indicating the minimum margins, in hundredths of an inch, the user is allowed to select.
        /// </summary>
        /// <returns>The minimum margins, in hundredths of an inch, the user is allowed to select. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Margins MinMargins
        {
            get => _minMargins;
            set => _minMargins = value ?? new Margins(0, 0, 0, 0);
        }
        /// <summary>
        /// Gets or sets a value indicating the page settings to modify.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Printing.PageSettings" /> to modify. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// Gets or sets the printer settings that are modified when the user clicks the Printer button in the dialog.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Printing.PrinterSettings" /> to modify when the user clicks the Printer button. The default is null.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
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
        /// Gets or sets a value indicating whether the Help button is visible.
        /// </summary>
        /// <returns>true if the Help button is visible; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public bool ShowHelp { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Network button is visible.
        /// </summary>
        /// <returns>true if the Network button is visible; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(true)]
        public bool ShowNetwork { get; set; }
        /// <summary>
        /// Resets all options to their default values.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
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

        protected override bool RunDialog(IWin32Window owner)
        {
            try
            {
                Gtk.Window window = Gtk.Window.ListToplevels().LastOrDefault(o => o is FormBase && o.IsActive);
                Gtk.PrintSettings printSettings = new Gtk.PrintSettings();
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
}
