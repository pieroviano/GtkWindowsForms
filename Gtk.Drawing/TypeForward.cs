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
[assembly: TypeForwardedTo(typeof(sdc::System.Drawing.StringUnit))]
