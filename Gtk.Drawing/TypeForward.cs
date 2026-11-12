extern alias sdc;
using System.Runtime.CompilerServices;

// Gtk.Drawing is a drop-in replacement for System.Drawing. Types it reimplements
// stay defined here; every remaining type of the System.Drawing surface is forwarded
// so consumers that reference only Gtk.Drawing still resolve the full API.
//
// Two resolution sources:
//   * Primitive value types live in the System.Drawing.Primitives framework facade
//     (part of the netstandard2.0 surface) and are referenced unaliased.
//   * Everything else is defined in System.Drawing.Common, referenced through the
//     `sdc` extern alias (see the Aliases="sdc" reference in Gtk.Drawing.csproj) to
//     avoid clashing with Gtk.Drawing's own System.Drawing.* types.

// System.Drawing.Primitives facade.
[assembly: TypeForwardedTo(typeof(System.Drawing.Color))]
[assembly: TypeForwardedTo(typeof(System.Drawing.Point))]
[assembly: TypeForwardedTo(typeof(System.Drawing.PointF))]
[assembly: TypeForwardedTo(typeof(System.Drawing.Rectangle))]
[assembly: TypeForwardedTo(typeof(System.Drawing.RectangleF))]
[assembly: TypeForwardedTo(typeof(System.Drawing.Size))]
[assembly: TypeForwardedTo(typeof(System.Drawing.SizeF))]

// System.Drawing.Common.
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.BitmapSuffixInSameAssemblyAttribute))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.BitmapSuffixInSatelliteAssemblyAttribute))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.ColorTranslator))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Design.CategoryNameCollection))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Drawing2D.AdjustableArrowCap))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Drawing2D.GraphicsPathIterator))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Drawing2D.PathPointType))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.IconConverter))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.Duplex))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.InvalidPrinterException))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.Margins))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.MarginsConverter))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PageSettings))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PaperKind))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PaperSize))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PaperSource))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PaperSourceKind))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PreviewPageInfo))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PreviewPrintController))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrintAction))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrintController))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrintDocument))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrintEventArgs))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrintEventHandler))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrintPageEventArgs))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrintPageEventHandler))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrintRange))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrinterResolution))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrinterResolutionKind))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrinterSettings))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrinterUnit))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.PrinterUnitConvert))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.QueryPageSettingsEventArgs))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.QueryPageSettingsEventHandler))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.Printing.StandardPrintController))]
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.StringUnit))]
