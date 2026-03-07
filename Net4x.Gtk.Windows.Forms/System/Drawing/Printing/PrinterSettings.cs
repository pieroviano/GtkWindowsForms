// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Drawing.Printing
{
    public partial class PrinterSettings : ICloneable
    {
        private byte[] _cachedDevmode;

        private short _copies = -1;

        private readonly PageSettings _defaultPageSettings;

        private ushort _devmodeBytes;

        private string _driverName = "";

        private ushort _extraBytes;

        private byte[] _extraInfo;

        private int _fromPage;

        private int _maxPage = 9999;

        private int _minPage;

        private string _printerName; // default printer.

        private PrintRange _printRange;

        private int _toPage;

        public PrinterSettings()
        {
            _defaultPageSettings = new PageSettings(this);
        }

        public bool CanDuplex
        {
            get;
            set;
        }

        public bool Collate
        {
            get;
            set;
        }

        public short Copies
        {
            get;
            set;
        }

        public PageSettings DefaultPageSettings => _defaultPageSettings;

        public Duplex Duplex
        {
            get;
            set;
        }

        public int FromPage
        {
            get => _fromPage;
            set
            {
                _fromPage = value;
            }
        }

        public static StringCollection InstalledPrinters
        {
            get;
            set;
        }

        public bool IsDefaultPrinter => false;

        public bool IsPlotter => false;

        public bool IsValid => true;

        public int LandscapeAngle => 0;

        public int MaximumCopies => 1;

        public int MaximumPage
        {
            get => _maxPage;
            set
            {
                _maxPage = value;
            }
        }

        public int MinimumPage
        {
            get => _minPage;
            set
            {
                _minPage = value;
            }
        }

        public string PrinterName
        {
            get;
            set;
        }

        public string PrintFileName
        {
            get;
            set;
        }

        public PrintRange PrintRange
        {
            get => _printRange;
            set
            {
                _printRange = value;
            }
        }

        public bool PrintToFile { get; set; }

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

        public object Clone()
        {
            PrinterSettings clone = (PrinterSettings)MemberwiseClone();

            return clone;
        }

        public bool IsDirectPrintingSupported(Image image)
        {
            ImageFormat imageFormat = image.RawFormat;

            if (!imageFormat.Equals(ImageFormat.Jpeg) && !imageFormat.Equals(ImageFormat.Png))
            {
                return false;
            }

            return false;
        }

        public void SetHdevmode(IntPtr hdevmode)
        {

        }

        public void SetHdevnames(IntPtr hdevnames)
        {

        }

        public override string ToString() =>
            $"[PrinterSettings {PrinterName} Copies={Copies} Collate={Collate} Duplex={Duplex} FromPage={FromPage} LandscapeAngle={LandscapeAngle} MaximumCopies={MaximumCopies} ToPage={ToPage}]";
    }
}
