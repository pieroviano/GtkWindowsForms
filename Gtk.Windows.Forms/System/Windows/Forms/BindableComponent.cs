// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Base class for a non-control component that supports data binding — it carries the
/// <see cref="DataBindings" /> collection and a <see cref="BindingContext" />, exactly as a
/// <see cref="Control" /> does, so a component can participate in binding without being a control. The type
/// was absent, so <c>class MyComponent : BindableComponent</c> did not compile.
/// </summary>
public abstract class BindableComponent : Component, IBindableComponent
{
    private BindingContext? _bindingContext;
    private ControlBindingsCollection? _dataBindings;

    /// <summary>Initializes a new instance of the <see cref="BindableComponent" /> class.</summary>
    protected BindableComponent()
    {
    }

    /// <summary>Occurs when the <see cref="BindingContext" /> changes.</summary>
    public event EventHandler? BindingContextChanged;

    /// <summary>Gets or sets the collection of currency managers for this component.</summary>
    public BindingContext? BindingContext
    {
        get => _bindingContext ??= new BindingContext();
        set
        {
            if (!Equals(_bindingContext, value))
            {
                _bindingContext = value;
                OnBindingContextChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>Gets the data bindings for this component.</summary>
    public ControlBindingsCollection? DataBindings => _dataBindings ??= new ControlBindingsCollection(this);

    /// <summary>Raises the <see cref="BindingContextChanged" /> event.</summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnBindingContextChanged(EventArgs e) => BindingContextChanged?.Invoke(this, e);
}
