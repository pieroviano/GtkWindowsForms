using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// IBindableComponent is the interface that must be implemented by any component that supports data binding. It provides the necessary properties to manage data bindings and currency managers for the component.
/// </summary>
/// <seealso cref="T:System.ComponentModel.IComponent" />
/// <seealso cref="T:System.IDisposable" />
public interface IBindableComponent : IComponent, IDisposable
{
    /// <summary>
    /// Gets or sets the collection of currency managers for the
    /// <see cref="T:System.Windows.Forms.IBindableComponent" /> .
    /// </summary>
    /// <returns>
    /// The collection of
    /// <see cref="T:System.Windows.Forms.BindingManagerBase" /> objects for this
    /// <see cref="T:System.Windows.Forms.IBindableComponent" />
    /// </returns>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    BindingContext BindingContext
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the collection of data-binding objects for this
    /// <see cref="T:System.Windows.Forms.IBindableComponent" /> .
    /// </summary>
    /// <returns>
    /// The
    /// <see cref="T:System.Windows.Forms.ControlBindingsCollection" />
    /// for this
    /// <see cref="T:System.Windows.Forms.IBindableComponent" /> .
    /// </returns>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    ControlBindingsCollection DataBindings { get; }
}