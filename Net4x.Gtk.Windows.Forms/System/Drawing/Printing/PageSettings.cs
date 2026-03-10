// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics;

namespace System.Drawing.Printing
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.ICloneable" />
    public unsafe class PageSettings : ICloneable
    {
        private Margins _margins = new Margins();

        private PaperSize _paperSize;

        private PaperSource _paperSource;

        private PrinterResolution _printerResolution;

        private PrinterSettings _printerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSettings"/> class.
        /// </summary>
        public PageSettings() : this(new PrinterSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PageSettings"/> class.
        /// </summary>
        /// <param name="printerSettings">The printer settings.</param>
        public PageSettings(PrinterSettings printerSettings)
        {
            _printerSettings = printerSettings;
        }

        /// <summary>
        /// Gets the bounds.
        /// </summary>
        /// <value>
        /// The bounds.
        /// </value>
        public Rectangle Bounds
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PageSettings"/> is color.
        /// </summary>
        /// <value>
        ///   <c>true</c> if color; otherwise, <c>false</c>.
        /// </value>
        public bool Color
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the hard margin x.
        /// </summary>
        /// <value>
        /// The hard margin x.
        /// </value>
        public float HardMarginX
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the hard margin y.
        /// </summary>
        /// <value>
        /// The hard margin y.
        /// </value>
        public float HardMarginY
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="PageSettings"/> is landscape.
        /// </summary>
        /// <value>
        ///   <c>true</c> if landscape; otherwise, <c>false</c>.
        /// </value>
        public bool Landscape
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the margins.
        /// </summary>
        /// <value>
        /// The margins.
        /// </value>
        public Margins Margins
        {
            get => _margins;
            set => _margins = value;
        }

        /// <summary>
        /// Gets or sets the size of the paper.
        /// </summary>
        /// <value>
        /// The size of the paper.
        /// </value>
        public PaperSize PaperSize
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the paper source.
        /// </summary>
        /// <value>
        /// The paper source.
        /// </value>
        public PaperSource PaperSource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the printable area.
        /// </summary>
        /// <value>
        /// The printable area.
        /// </value>
        public RectangleF PrintableArea
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the printer resolution.
        /// </summary>
        /// <value>
        /// The printer resolution.
        /// </value>
        public PrinterResolution PrinterResolution
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the printer settings.
        /// </summary>
        /// <value>
        /// The printer settings.
        /// </value>
        public PrinterSettings PrinterSettings
        {
            get => _printerSettings;
            set => _printerSettings = value ?? new PrinterSettings();
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            PageSettings result = (PageSettings)MemberwiseClone();
            result._margins = (Margins)_margins.Clone();

            return result;
        }

        /// <summary>
        /// Copies to hdevmode.
        /// </summary>
        /// <param name="hdevmode">The hdevmode.</param>
        public void CopyToHdevmode(IntPtr hdevmode)
        {

        }

        /// <summary>
        /// Sets the hdevmode.
        /// </summary>
        /// <param name="hdevmode">The hdevmode.</param>
        public void SetHdevmode(IntPtr hdevmode)
        {

        }

        /// <summary>
        /// Converts to string.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() =>
            $"[{nameof(PageSettings)}: Color={Color}, Landscape={Landscape}, Margins={Margins}, PaperSize={PaperSize}, PaperSource={PaperSource}, PrinterResolution={PrinterResolution}]";

    }
}
