// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms;

/// <summary>
/// Represents the collection of data bindings for a control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
[DefaultEvent(nameof(CollectionChanged))]
[TypeConverter($"System.Windows.Forms.Design.ControlBindingsConverter, {AssemblyRef.SystemDesign}")]
public class ControlBindingsCollection : BindingsCollection
{
    private readonly IBindableComponent? _control;

    /// <summary>
    /// Initializes a new instance of the <see cref="ControlBindingsCollection"/> class for the specified bindable component.
    /// </summary>
    /// <param name="control">The control (or component) that will own bindings added to this collection.</param>
    public ControlBindingsCollection(IBindableComponent? control)
    {
        _control = control;
    }

    /// <summary>
    /// Gets the component that is bindable and associated with this collection.
    /// This can be null for an unbound collection.
    /// </summary>
    public IBindableComponent? BindableComponent => _control;

    /// <summary>
    /// Gets the underlying <see cref="Control"/> instance if the bindable component is a Control; otherwise null.
    /// </summary>
    public Control? Control => _control as Control;

    /// <summary>
    /// Gets or sets the default data source update mode used when creating new <see cref="Binding"/> instances.
    /// </summary>
    public DataSourceUpdateMode DefaultDataSourceUpdateMode { get; set; } = DataSourceUpdateMode.OnValidation;

    /// <summary>
    /// Gets the first <see cref="Binding"/> in the collection that targets the specified property name,
    /// or null if no binding for that property is present.
    /// Comparison is case-insensitive.
    /// </summary>
    /// <param name="propertyName">The name of the property on the control to find the binding for.</param>
    public Binding? this[string propertyName]
    {
        get
        {
            foreach (Binding binding in this)
            {
                if (string.Equals(binding.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return binding;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Adds the specified <see cref="Binding"/> to the collection.
    /// Throws if <paramref name="binding"/> is null or invalid for this control.
    /// </summary>
    /// <param name="binding">The binding to add.</param>
    public new void Add(Binding binding) => base.Add(binding);

    /// <summary>
    /// Creates a <see cref="Binding"/> that binds the specified property to the given data source and adds it to the collection.
    /// Uses the default formatting and update mode.
    /// </summary>
    /// <param name="propertyName">The property on the control to bind.</param>
    /// <param name="dataSource">The data source object.</param>
    /// <param name="dataMember">The data member (column/property) on the data source to bind to.</param>
    /// <returns>The created and added <see cref="Binding"/>.</returns>
    public Binding Add(string propertyName, object dataSource, string? dataMember) =>
        Add(
            propertyName,
            dataSource,
            dataMember,
            formattingEnabled: false,
            DefaultDataSourceUpdateMode,
            nullValue: null,
            formatString: string.Empty,
            formatInfo: null);

    /// <summary>
    /// Creates and adds a binding with the specified formatting option.
    /// </summary>
    public Binding Add(
        string propertyName,
        object dataSource,
        string? dataMember,
        bool formattingEnabled) =>
            Add(
                propertyName,
                dataSource,
                dataMember,
                formattingEnabled,
                DefaultDataSourceUpdateMode,
                nullValue: null,
                formatString: string.Empty,
                formatInfo: null);

    /// <summary>
    /// Creates and adds a binding with the specified formatting and update mode.
    /// </summary>
    public Binding Add(
        string propertyName,
        object dataSource,
        string? dataMember,
        bool formattingEnabled,
        DataSourceUpdateMode updateMode) =>
            Add(
                propertyName,
                dataSource,
                dataMember,
                formattingEnabled,
                updateMode,
                nullValue: null,
                formatString: string.Empty,
                formatInfo: null);

    /// <summary>
    /// Creates and adds a binding with the specified null-value substitution.
    /// </summary>
    public Binding Add(
        string propertyName,
        object dataSource,
        string? dataMember,
        bool formattingEnabled,
        DataSourceUpdateMode updateMode,
        object? nullValue) =>
            Add(
                propertyName,
                dataSource,
                dataMember,
                formattingEnabled,
                updateMode,
                nullValue,
                formatString: string.Empty,
                formatInfo: null);

    /// <summary>
    /// Creates and adds a binding with a format string.
    /// </summary>
    public Binding Add(
        string propertyName,
        object dataSource,
        string? dataMember,
        bool formattingEnabled,
        DataSourceUpdateMode updateMode,
        object? nullValue,
        string formatString) =>
            Add(
                propertyName,
                dataSource,
                dataMember,
                formattingEnabled,
                updateMode,
                nullValue,
                formatString,
                formatInfo: null);

    /// <summary>
    /// Creates and adds a binding with full formatting and format provider options.
    /// </summary>
    /// <exception cref="ArgumentNullException">If <paramref name="dataSource"/> is null.</exception>
    public Binding Add(
        string propertyName,
        object dataSource,
        string? dataMember,
        bool formattingEnabled,
        DataSourceUpdateMode updateMode,
        object? nullValue,
        string formatString,
        IFormatProvider? formatInfo)
    {
        if (dataSource == null) throw new ArgumentNullException(nameof(dataSource));

        Binding binding = new(
            propertyName,
            dataSource,
            dataMember,
            formattingEnabled,
            updateMode,
            nullValue,
            formatString,
            formatInfo);
        Add(binding);

        return binding;
    }

    /// <summary>
    /// Removes all bindings from the collection.
    /// The CollectionChanged event is raised.
    /// </summary>
    public new void Clear() => base.Clear();

    /// <summary>
    /// Removes the specified binding from the collection.
    /// Throws if the binding is null or does not belong to this collection.
    /// </summary>
    /// <param name="binding">The binding to remove.</param>
    public new void Remove(Binding binding) => base.Remove(binding);

    /// <summary>
    /// Removes the binding at the specified index.
    /// </summary>
    /// <param name="index">Zero-based index of the binding to remove.</param>
    public new void RemoveAt(int index) => base.RemoveAt(index);

    /// <summary>
    /// Verifies that no existing binding in the collection targets the same property as the provided binding.
    /// Throws <see cref="ArgumentException"/> when a duplicate is detected.
    /// </summary>
    /// <param name="binding">Binding to check for duplicates.</param>
    internal void CheckDuplicates(Binding binding)
    {
        Debug.Assert(!string.IsNullOrEmpty(binding.PropertyName), "The caller should check for this.");

        for (var i = 0; i < Count; i++)
        {
            var current = this[i];
            if (binding != current
                && !string.IsNullOrEmpty(current.PropertyName)
                && string.Equals(binding.PropertyName, current.PropertyName, StringComparison.InvariantCulture))
            {
                throw new ArgumentException(nameof(binding));
            }
        }
    }

    /// <summary>
    /// Core add implementation. Validates and associates the binding with this collection's component,
    /// then delegates to the base implementation to insert it.
    /// </summary>
    /// <param name="dataBinding">The binding to add.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="dataBinding"/> is null.</exception>
    /// <exception cref="ArgumentException">If the binding is already associated with another component.</exception>
    protected override void AddCore(Binding dataBinding)
    {
        if (dataBinding == null) throw new ArgumentNullException(nameof(dataBinding));

        if (dataBinding.BindableComponent == _control)
        {
            throw new ArgumentException(nameof(dataBinding));
        }

        if (dataBinding.BindableComponent is not null)
        {
            throw new ArgumentException(nameof(dataBinding));
        }

        // important to set prop first for error checking.
        dataBinding.SetBindableComponent(_control);

        base.AddCore(dataBinding);
    }

    /// <summary>
    /// Core clear implementation. Disassociates all bindings from the owning component before clearing.
    /// </summary>
    protected override void ClearCore()
    {
        var numLinks = Count;
        for (var i = 0; i < numLinks; i++)
        {
            var dataBinding = this[i];
            dataBinding.SetBindableComponent(null);
        }

        base.ClearCore();
    }

    /// <summary>
    /// Core remove implementation. Validates and disassociates the binding before removing it from the collection.
    /// </summary>
    /// <param name="dataBinding">The binding to remove.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="dataBinding"/> is null.</exception>
    /// <exception cref="ArgumentException">If the binding does not belong to this collection.</exception>
    protected override void RemoveCore(Binding dataBinding)
    {
        if (dataBinding == null) throw new ArgumentNullException(nameof(dataBinding));

        if (dataBinding.BindableComponent != _control)
        {
            throw new ArgumentException(nameof(dataBinding));
        }

        dataBinding.SetBindableComponent(value: null);
        base.RemoveCore(dataBinding);
    }

}

