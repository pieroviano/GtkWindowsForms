using System.ComponentModel;

namespace System.Windows.Forms
{
    public interface IBindableComponent : IComponent, IDisposable
    {
        /// <summary>
        /// Gets the collection of data-binding objects for this
        /// <see cref="System.Windows.Forms.IBindableComponent" /> .
        /// </summary>
        /// <returns>
        /// The
        /// <see cref="System.Windows.Forms.ControlBindingsCollection" />
        /// for this
        /// <see cref="System.Windows.Forms.IBindableComponent" /> .
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        ControlBindingsCollection DataBindings { get; }
        /// <summary>
        /// Gets or sets the collection of currency managers for the
        /// <see cref="System.Windows.Forms.IBindableComponent" /> .
        /// </summary>
        /// <returns>
        /// The collection of
        /// <see cref="System.Windows.Forms.BindingManagerBase" /> objects for this
        /// <see cref="System.Windows.Forms.IBindableComponent" />
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        BindingContext BindingContext { get; set; }
    }
}
