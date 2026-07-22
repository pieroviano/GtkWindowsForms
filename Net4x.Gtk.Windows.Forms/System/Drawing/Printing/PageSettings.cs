// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
/// </summary>
/// <seealso cref="T:System.ICloneable" />
public unsafe class PageSettings : ICloneable
{
    private Margins _margins = new();

    private PaperSize? _paperSize;

    private PaperSource? _paperSource;

    private PrinterResolution? _printerResolution;

    private PrinterSettings _printerSettings;

    private RectangleF _printableArea;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Printing.PageSettings" /> class.
    /// </summary>
    public PageSettings() : this(new PrinterSettings())
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Printing.PageSettings" /> class.
    /// </summary>
    /// <param name="printerSettings">
    /// The printer settings.
    /// </param>
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
        // This was a stored value nothing ever set, so it stayed empty. WinForms computes the page
        // bounds from the paper size, swapping the axes in landscape orientation; margins are not
        // applied.
        get
        {
            var size = PaperSize;

            return Landscape
                ? new Rectangle(0, 0, size.Height, size.Width)
                : new Rectangle(0, 0, size.Width, size.Height);
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Drawing.Printing.PageSettings" /> is color.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if color; otherwise,
    /// <c>
    /// false
    /// </c>.
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
    /// Gets or sets a value indicating whether this
    /// <see cref="T:System.Drawing.Printing.PageSettings" /> is landscape.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if landscape; otherwise,
    /// <c>
    /// false
    /// </c>.
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
        // WinForms never returns null here - the value defaults from the printer. Without a real
        // printer the shim defaults to Letter (850 x 1100 hundredths of an inch).
        get => _paperSize ??= new PaperSize(PaperKind.Letter, "Letter", 850, 1100);
        set => _paperSize = value;
    }

    /// <summary>
    /// Gets or sets the paper source.
    /// </summary>
    /// <value>
    /// The paper source.
    /// </value>
    public PaperSource PaperSource
    {
        get => _paperSource ??= new PaperSource(PaperSourceKind.FormSource, "FormSource");
        set => _paperSource = value;
    }

    /// <summary>
    /// Gets the printable area.
    /// </summary>
    /// <value>
    /// The printable area.
    /// </value>
    public RectangleF PrintableArea
    {
        // A virtual page has no hard margins, so the whole (portrait) page is printable unless the
        // print pipeline supplies a tighter area.
        get => _printableArea.IsEmpty
            ? new RectangleF(0, 0, PaperSize.Width, PaperSize.Height)
            : _printableArea;

        internal set => _printableArea = value;
    }

    /// <summary>
    /// Gets or sets the printer resolution.
    /// </summary>
    /// <value>
    /// The printer resolution.
    /// </value>
    public PrinterResolution PrinterResolution
    {
        get => _printerResolution ??= new PrinterResolution(PrinterResolutionKind.Custom, 200, 200);
        set => _printerResolution = value;
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
        var result = (PageSettings)MemberwiseClone();
        result._margins = (Margins)_margins.Clone();

        return result;
    }

    /// <summary>
    /// Copies to hdevmode.
    /// </summary>
    /// <param name="hdevmode">
    /// The hdevmode.
    /// </param>
    public void CopyToHdevmode(IntPtr hdevmode)
    {

    }

    /// <summary>
    /// Sets the hdevmode.
    /// </summary>
    /// <param name="hdevmode">
    /// The hdevmode.
    /// </param>
    public void SetHdevmode(IntPtr hdevmode)
    {

    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() =>
                $"[{nameof(PageSettings)}: Color={Color}, Landscape={Landscape}, Margins={Margins}, PaperSize={PaperSize}, PaperSource={PaperSource}, PrinterResolution={PrinterResolution}]";

}