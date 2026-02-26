using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// Provides a base class for property tabs.
    /// </summary>
    public abstract class PropertyTab : IExtenderProvider
    {

        private Bitmap? _bitmap;
        private bool _checkedBitmap;
        /// <summary>
        /// Gets the bitmap that is displayed for the
        /// <see cref="System.Windows.Forms.Design.PropertyTab" />
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Bitmap" /> to display for the
        /// <see cref="System.Windows.Forms.Design.PropertyTab" />
        /// </returns>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public virtual Bitmap? Bitmap
        {
            get
            {
                if (!_checkedBitmap && _bitmap is null)
                {

                    _checkedBitmap = true;
                }

                return _bitmap;
            }
        }
        /// <summary>
        /// Gets or sets the array of components the property tab is associated with.
        /// </summary>
        /// <returns>The array of components the property tab is associated with.
        /// </returns>
        public virtual object[]? Components { get; set; }
        /// <summary>
        /// Gets the name for the property tab.
        /// </summary>
        /// <returns>The name for the property tab.
        /// </returns>
        public abstract string? TabName { get; }
        /// <summary>
        /// Gets the Help keyword that is to be associated with this tab.
        /// </summary>
        /// <returns>The Help keyword to be associated with this tab.
        /// </returns>
        public virtual string? HelpKeyword => TabName;
        /// <summary>
        /// Gets a value indicating whether this
        /// <see cref="System.Windows.Forms.Design.PropertyTab" /> can display properties for the specified component.
        /// </summary>
        /// <returns>true if the object can be extended; otherwise, false.
        /// </returns>
        /// <param name="extendee">The object to test.
        /// </param>
        public virtual bool CanExtend(object? extendee) => true;
        /// <summary>
        /// Releases all the resources used by the
        /// <see cref="System.Windows.Forms.Design.PropertyTab" />
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Releases the unmanaged resources used by the
        /// <see cref="System.Windows.Forms.Design.PropertyTab" /> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bitmap?.Dispose();
                _bitmap = null;
            }
        }

        ~PropertyTab() => Dispose(disposing: false);
        /// <summary>
        /// Gets the default property of the specified component.
        /// </summary>
        /// <returns>A
        /// <see cref="System.ComponentModel.PropertyDescriptor" /> that represents the default property.
        /// </returns>
        /// <param name="component">The component to retrieve the default property of.
        /// </param>
        public virtual PropertyDescriptor? GetDefaultProperty(object component)
                    => TypeDescriptor.GetDefaultProperty(component);
        /// <summary>
        /// Gets the properties of the specified component.
        /// </summary>
        /// <returns>A
        /// <see cref="System.ComponentModel.PropertyDescriptorCollection" /> that contains the properties of the component.
        /// </returns>
        /// <param name="component">The component to retrieve the properties of.
        /// </param>
        public virtual PropertyDescriptorCollection? GetProperties(object component)
                    => GetProperties(component, attributes: null);
        /// <summary>
        /// Gets the properties of the specified component that match the specified attributes.
        /// </summary>
        /// <returns>A
        /// <see cref="System.ComponentModel.PropertyDescriptorCollection" /> that contains the properties.
        /// </returns>
        /// <param name="component">The component to retrieve properties from.
        /// </param>
        /// <param name="attributes">An array of type
        /// <see cref="System.Attribute" /> that indicates the attributes of the properties to retrieve.
        /// </param>
        public abstract PropertyDescriptorCollection? GetProperties(object component, Attribute[]? attributes);
        /// <summary>
        /// Gets the properties of the specified component that match the specified attributes and context.
        /// </summary>
        /// <returns>A
        /// <see cref="System.ComponentModel.PropertyDescriptorCollection" /> that contains the properties matching the specified context and attributes.
        /// </returns>
        /// <param name="context">An
        /// <see cref="System.ComponentModel.ITypeDescriptorContext" /> that indicates the context to retrieve properties from.
        /// </param>
        /// <param name="component">The component to retrieve properties from.
        /// </param>
        /// <param name="attributes">An array of type
        /// <see cref="System.Attribute" /> that indicates the attributes of the properties to retrieve.
        /// </param>
        public virtual PropertyDescriptorCollection? GetProperties(
                    ITypeDescriptorContext? context,
                    object component,
                    Attribute[]? attributes) => GetProperties(component, attributes);
    }
}
