using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides a type converter to convert
    /// <see cref="System.Windows.Forms.ListViewItem" />
    /// objects to and from various other representations.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ListViewItemConverter : ExpandableObjectConverter
    {
        /// <summary>
        /// Gets a value indicating whether this converter can convert an object to the given destination type using the context.
        /// </summary>
        /// <returns>true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.
        /// </param>
        /// <param name="destinationType">A
        /// <see cref="System.Type" /> that represents the type you wish to convert to.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            throw new System.NotImplementedException();
        }
        /// <summary>
        /// Converts the given object to another type.
        /// </summary>
        /// <returns>The converted object.
        /// </returns>
        /// <param name="context">A formatter context. This object can be used to extract additional information about the environment this converter is being invoked from. This may be null, so you should always check. Also, properties on the context object may also return null.
        /// </param>
        /// <param name="culture">An optional culture info. If not supplied the current culture is assumed.
        /// </param>
        /// <param name="value">The object to convert.
        /// </param>
        /// <param name="destinationType">The type to convert the object to.
        /// </param>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            throw new System.NotImplementedException();
        }

        public ListViewItemConverter()
        {
            throw new System.NotImplementedException();
        }
    }
}
