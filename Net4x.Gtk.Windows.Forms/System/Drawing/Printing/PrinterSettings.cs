// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing.Imaging;

namespace System.Drawing.Printing;

/// <summary>
/// Represents the settings of a printer, including page ranges, copies and device-specific information.
/// This is a lightweight, cross-platform implementation used by the GTK-based windows-forms shim.
/// </summary>
public partial class PrinterSettings : ICloneable
{
    private readonly byte[] _cachedDevmode;

    private readonly short _copies = -1;

    private readonly PageSettings _defaultPageSettings;

    private readonly ushort _devmodeBytes;

    private readonly string _driverName = "";

    private readonly ushort _extraBytes;

    private readonly byte[] _extraInfo;

    private int _fromPage;

    private int _maxPage = 9999;

    private int _minPage;

    private readonly string _printerName; // default printer.

    private PrintRange _printRange;

    private int _toPage;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Printing.PrinterSettings" /> class.
    /// </summary>
    public PrinterSettings()
    {
        _defaultPageSettings = new PageSettings(this);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the printer supports duplex (double-sided) printing.
    /// </summary>
    public bool CanDuplex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether multiple copies should be collated.
    /// </summary>
    public bool Collate
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the number of copies to print.
    /// </summary>
    public short Copies
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the default page settings for this printer.
    /// </summary>
    public PageSettings DefaultPageSettings => _defaultPageSettings;

    /// <summary>
    /// Gets or sets the duplex mode for the printer.
    /// </summary>
    public Duplex Duplex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the first page to print in a range.
    /// </summary>
    public int FromPage
    {
        get => _fromPage;
        set
        {
            _fromPage = value;
        }
    }

    /// <summary>
    /// Gets the collection of installed printer names. Implementation/platform-specific.
    /// </summary>
    public static StringCollection InstalledPrinters
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether this printer is the system default printer.
    /// </summary>
    public bool IsDefaultPrinter => false;

    /// <summary>
    /// Gets a value indicating whether this printer is a plotter.
    /// </summary>
    public bool IsPlotter => false;

    /// <summary>
    /// Gets a value indicating whether the current printer settings are valid.
    /// </summary>
    public bool IsValid => true;

    /// <summary>
    /// Gets the landscape angle supported by the printer (degrees).
    /// </summary>
    public int LandscapeAngle => 0;

    /// <summary>
    /// Gets the maximum number of copies supported by the printer.
    /// </summary>
    public int MaximumCopies => 1;

    /// <summary>
    /// Gets or sets the maximum page number that can be printed.
    /// </summary>
    public int MaximumPage
    {
        get => _maxPage;
        set
        {
            _maxPage = value;
        }
    }

    /// <summary>
    /// Gets or sets the minimum page number that can be printed.
    /// </summary>
    public int MinimumPage
    {
        get => _minPage;
        set
        {
            _minPage = value;
        }
    }

    /// <summary>
    /// Gets or sets the printer name associated with these settings.
    /// </summary>
    public string PrinterName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the file name to use when printing to a file.
    /// </summary>
    public string PrintFileName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the range of pages to print.
    /// </summary>
    public PrintRange PrintRange
    {
        get => _printRange;
        set
        {
            _printRange = value;
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether output should be printed to a file.
    /// </summary>
    public bool PrintToFile { get; set; }

    /// <summary>
    /// Gets or sets the last page to print in a range.
    /// </summary>
    public int ToPage
    {
        get => _toPage;
        set
        {
            _toPage = value;
        }
    }

    // As far as I can tell, Windows no longer pays attention to driver names and output ports.
    // But I'm leaving this code in place in case I'm wrong.
    internal string DriverName => _driverName;

    /// <summary>
    /// Creates a shallow copy of the current
    /// <see cref="T:System.Drawing.Printing.PrinterSettings" />.
    /// </summary>
    /// <returns>
    /// A new
    /// <see cref="T:System.Drawing.Printing.PrinterSettings" /> instance with the same values.
    /// </returns>
    public object Clone()
    {
        var clone = (PrinterSettings)MemberwiseClone();

        return clone;
    }

    /// <summary>
    /// Determines whether direct printing of the supplied image format is supported by this implementation.
    /// </summary>
    /// <param name="image">
    /// The image to test.
    /// </param>
    /// <returns>
    /// True if the image format can be printed directly; otherwise false.
    /// </returns>
    public bool IsDirectPrintingSupported(Image image)
    {
        ImageFormat imageFormat = image.RawFormat;

        if (!imageFormat.Equals(ImageFormat.Jpeg) && !imageFormat.Equals(ImageFormat.Png))
        {
            return false;
        }

        return false;
    }

    /// <summary>
    /// Sets the handle to a DEVMODE structure that contains device settings for the printer.
    /// Platform-specific; no-op in this implementation.
    /// </summary>
    /// <param name="hdevmode">
    /// Pointer to a DEVMODE structure.
    /// </param>
    public void SetHdevmode(IntPtr hdevmode)
    {

    }

    /// <summary>
    /// Sets the handle to a DEVNAMES structure that contains device and driver names.
    /// Platform-specific; no-op in this implementation.
    /// </summary>
    /// <param name="hdevnames">
    /// Pointer to a DEVNAMES structure.
    /// </param>
    public void SetHdevnames(IntPtr hdevnames)
    {

    }

    /// <summary>
    /// Returns a string that represents the current printer settings.
    /// </summary>
    /// <returns>
    /// A string with selected printer settings.
    /// </returns>
    public override string ToString() =>
                $"[PrinterSettings {PrinterName} Copies={Copies} Collate={Collate} Duplex={Duplex} FromPage={FromPage} LandscapeAngle={LandscapeAngle} MaximumCopies={MaximumCopies} ToPage={ToPage}]";

}