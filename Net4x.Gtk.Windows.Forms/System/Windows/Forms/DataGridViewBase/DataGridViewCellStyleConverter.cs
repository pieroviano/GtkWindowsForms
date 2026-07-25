// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
/// Converts <see cref="DataGridViewCellStyle" /> objects for the designer. It was absent, so a
/// <c>[TypeConverter(typeof(DataGridViewCellStyleConverter))]</c> annotation did not compile. The
/// conversion surface is inherited from <see cref="TypeConverter" />; the designer-serialization
/// (<see cref="InstanceDescriptor" />) path is advertised but produces no descriptor on this layer.
/// </summary>
public class DataGridViewCellStyleConverter : TypeConverter
{
    /// <summary>Initializes a new instance of the <see cref="DataGridViewCellStyleConverter" /> class.</summary>
    public DataGridViewCellStyleConverter()
    {
    }

    /// <inheritdoc />
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
        destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);

    /// <inheritdoc />
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == null)
        {
            throw new ArgumentNullException(nameof(destinationType));
        }

        // Designer serialization to an InstanceDescriptor has no representation on this layer; other
        // conversions (notably to string) defer to the base converter.
        return base.ConvertTo(context, culture, value, destinationType);
    }
}
