using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
/// Provides a base class for property tabs.
/// </summary>
public abstract class PropertyTab : IExtenderProvider
{

    private Bitmap? _bitmap;

    private bool _checkedBitmap;

    /// <summary>
    /// Gets the bitmap.
    /// </summary>
    /// <value>
    /// The bitmap.
    /// </value>
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
    /// Gets or sets the components.
    /// </summary>
    /// <value>
    /// The components.
    /// </value>
    public virtual object[]? Components
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the help keyword.
    /// </summary>
    /// <value>
    /// The help keyword.
    /// </value>
    public virtual string? HelpKeyword => TabName;

    /// <summary>
    /// Gets the name of the tab.
    /// </summary>
    /// <value>
    /// The name of the tab.
    /// </value>
    public abstract string? TabName { get; }

    /// <summary>
    /// Specifies whether this object can provide its extender properties to the specified object.
    /// </summary>
    /// <param name="extendee">
    /// The
    /// <see cref="T:System.Object">
    /// </see> to receive the extender properties.
    /// </param>
    /// <returns>
    /// true if this object can provide extender properties to the specified object; otherwise, false.
    /// </returns>
    public virtual bool CanExtend(object? extendee) => true;

    /// <summary>
    /// Disposes this instance.
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Gets the default property.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    /// <returns>
    /// System.Nullable{PropertyDescriptor}.
    /// </returns>
    public virtual PropertyDescriptor? GetDefaultProperty(object component)
    {
        // First try the standard TypeDescriptor lookup.
        var prop = TypeDescriptor.GetDefaultProperty(component);
        if (prop is not null)
        {
            return prop;
        }

        // Fallback behaviour:
        // - If no DefaultPropertyAttribute is defined, prefer a property named "Name"
        //   (common convention for components), otherwise return the first property.
        var props = TypeDescriptor.GetProperties(component);
        if (props is null || props.Count == 0)
        {
            return null;
        }

        var nameProp = props["Name"];
        if (nameProp is not null)
        {
            return nameProp;
        }

        return props[0];
    }

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    /// <returns>
    /// System.Nullable{PropertyDescriptorCollection}.
    /// </returns>
    public virtual PropertyDescriptorCollection? GetProperties(object component)
                => GetProperties(component, attributes: null);

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <param name="component">
    /// The component.
    /// </param>
    /// <param name="attributes">
    /// The attributes.
    /// </param>
    /// <returns>
    /// System.Nullable{PropertyDescriptorCollection}.
    /// </returns>
    public abstract PropertyDescriptorCollection? GetProperties(object component, Attribute[]? attributes);

    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <param name="context">
    /// The context.
    /// </param>
    /// <param name="component">
    /// The component.
    /// </param>
    /// <param name="attributes">
    /// The attributes.
    /// </param>
    /// <returns>
    /// System.Nullable{PropertyDescriptorCollection}.
    /// </returns>
    public virtual PropertyDescriptorCollection? GetProperties(
                ITypeDescriptorContext? context,
                object component,
                Attribute[]? attributes) => GetProperties(component, attributes);

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing">
    /// <c>
    /// true
    /// </c> to release both managed and unmanaged resources;
    /// <c>
    /// false
    /// </c> to release only unmanaged resources.
    /// </param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _bitmap?.Dispose();
            _bitmap = null;
        }
    }
}