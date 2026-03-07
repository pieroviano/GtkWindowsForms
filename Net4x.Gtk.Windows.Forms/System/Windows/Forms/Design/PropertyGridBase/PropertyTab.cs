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

        public virtual object[]? Components { get; set; }

        public virtual string? HelpKeyword => TabName;

        public abstract string? TabName { get; }

        public virtual bool CanExtend(object? extendee) => true;

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual PropertyDescriptor? GetDefaultProperty(object component)
            => TypeDescriptor.GetDefaultProperty(component);

        public virtual PropertyDescriptorCollection? GetProperties(object component)
            => GetProperties(component, attributes: null);

        public abstract PropertyDescriptorCollection? GetProperties(object component, Attribute[]? attributes);

        public virtual PropertyDescriptorCollection? GetProperties(
            ITypeDescriptorContext? context,
            object component,
            Attribute[]? attributes) => GetProperties(component, attributes);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bitmap?.Dispose();
                _bitmap = null;
            }
        }
    }
}
