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
    /// Initializes a new instance of the
    /// <see cref="System.Windows.Forms.ControlBindingsCollection" /> class with the specified bindable control.
    /// </summary>
    /// <param name="control">The
    /// <see cref="System.Windows.Forms.IBindableComponent" /> the binding collection belongs to.
    /// </param>
    public ControlBindingsCollection(IBindableComponent? control)
    {
        _control = control;
    }

    /// <summary>
    /// Gets the
    /// <see cref="System.Windows.Forms.IBindableComponent" /> the binding collection belongs to.
    /// </summary>
    /// <returns>The
    /// <see cref="System.Windows.Forms.IBindableComponent" /> the binding collection belongs to.
    /// </returns>
    /// <filterpriority>1
    /// </filterpriority>
    public IBindableComponent? BindableComponent => _control;

    /// <summary>
    /// Gets the control that the collection belongs to.
    /// </summary>
    /// <returns>The
    /// <see cref="System.Windows.Forms.Control" /> that the collection belongs to.
    /// </returns>
    /// <filterpriority>1
    /// </filterpriority>
    public Control? Control => _control as Control;

    /// <summary>
    /// Gets or sets the default
    /// <see cref="System.Windows.Forms.Binding.DataSourceUpdateMode" /> for a
    /// <see cref="System.Windows.Forms.Binding" /> in the collection.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DataSourceUpdateMode" /> values.
    /// </returns>
    public DataSourceUpdateMode DefaultDataSourceUpdateMode { get; set; } = DataSourceUpdateMode.OnValidation;

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
    /// Adds the specified
    /// <see cref="System.Windows.Forms.Binding" /> to the collection.
    /// </summary>
    /// <param name="binding">The
    /// <see cref="System.Windows.Forms.Binding" /> to add.
    /// </param>
    /// <exception cref="System.ArgumentNullException">The
    /// <paramref name="binding" /> is null.
    /// </exception>
    /// <exception cref="System.ArgumentException">The control property is already data-bound.
    /// </exception>
    /// <exception cref="System.ArgumentException">The
    /// <see cref="System.Windows.Forms.Binding" /> does not specify a valid column of the
    /// <see cref="System.Windows.Forms.Binding.DataSource" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
    public new void Add(Binding binding) => base.Add(binding);

    /// <summary>
    /// Creates a
    /// <see cref="System.Windows.Forms.Binding" /> using the specified control property name, data source, and data member, and adds it to the collection.
    /// </summary>
    /// <returns>The newly created
    /// <see cref="System.Windows.Forms.Binding" />
    /// </returns>
    /// <param name="propertyName">The name of the control property to bind.
    /// </param>
    /// <param name="dataSource">An
    /// <see cref="System.Object" /> that represents the data source.
    /// </param>
    /// <param name="dataMember">The property or list to bind to.
    /// </param>
    /// <exception cref="System.ArgumentNullException">The
    /// <paramref name="binding" /> is null.
    /// </exception>
    /// <exception cref="System.Exception">The
    /// <paramref name="propertyName" /> is already data-bound.
    /// </exception>
    /// <exception cref="System.Exception">The
    /// <paramref name="dataMember" /> doesn't specify a valid member of the
    /// <paramref name="dataSource" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
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
    /// Creates a binding with the specified control property name, data source, data member, and information about whether formatting is enabled, and adds the binding to the collection.
    /// </summary>
    /// <returns>The newly created
    /// <see cref="System.Windows.Forms.Binding" />
    /// </returns>
    /// <param name="propertyName">The name of the control property to bind.
    /// </param>
    /// <param name="dataSource">An
    /// <see cref="System.Object" /> representing the data source.
    /// </param>
    /// <param name="dataMember">The property or list to bind to.
    /// </param>
    /// <param name="formattingEnabled">true to format the displayed data; otherwise, false
    /// </param>
    /// <exception cref="System.ArgumentException">The property given by
    /// <paramref name="propertyName" /> does not exist on the control.-or-The property given is a read-only property.
    /// </exception>
    /// <exception cref="System.Exception">If formatting is disabled and the
    /// <paramref name="propertyName" /> is neither a valid property of a control nor an empty string ("").
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
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
    /// Creates a binding that binds the specified control property to the specified data member of the specified data source, optionally enabling formatting, propagating values to the data source based on the specified update setting, and adding the binding to the collection.
    /// </summary>
    /// <returns>The newly created
    /// <see cref="System.Windows.Forms.Binding" />
    /// </returns>
    /// <param name="propertyName">The name of the control property to bind.
    /// </param>
    /// <param name="dataSource">An
    /// <see cref="System.Object" /> representing the data source.
    /// </param>
    /// <param name="dataMember">The property or list to bind to.
    /// </param>
    /// <param name="formattingEnabled">true to format the displayed data; otherwise, false.
    /// </param>
    /// <param name="updateMode">One of the
    /// <see cref="System.Windows.Forms.DataSourceUpdateMode" /> values.
    /// </param>
    /// <exception cref="System.ArgumentException">The property given by
    /// <paramref name="propertyName" /> does not exist on the control or is read-only.-or-The specified data member does not exist on the data source.-or-The data source, data member, or control property specified are associated with another binding in the collection.
    /// </exception>
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
    /// Creates a binding that binds the specified control property to the specified data member of the specified data source, optionally enabling formatting, propagating values to the data source based on the specified update setting, setting the property to the specified value when
    /// <see cref="System.DBNull" /> is returned from the data source, and adding the binding to the collection.
    /// </summary>
    /// <returns>The newly created
    /// <see cref="System.Windows.Forms.Binding" />
    /// </returns>
    /// <param name="propertyName">The name of the control property to bind.
    /// </param>
    /// <param name="dataSource">An
    /// <see cref="System.Object" /> representing the data source.
    /// </param>
    /// <param name="dataMember">The property or list to bind to.
    /// </param>
    /// <param name="formattingEnabled">true to format the displayed data; otherwise, false.
    /// </param>
    /// <param name="updateMode">One of the
    /// <see cref="System.Windows.Forms.DataSourceUpdateMode" /> values.
    /// </param>
    /// <param name="nullValue">The
    /// <see cref="System.Object" /> to be applied to the bound control property if the data source value is
    /// <see cref="System.DBNull" />
    /// </param>
    /// <exception cref="System.ArgumentException">The property given by
    /// <paramref name="propertyName" /> does not exist on the control or is read-only.-or-The specified data member does not exist on the data source.-or-The data source, data member, or control property specified are associated with another binding in the collection.
    /// </exception>
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
    /// Creates a binding that binds the specified control property to the specified data member of the specified data source, optionally enabling formatting with the specified format string, propagating values to the data source based on the specified update setting, setting the property to the specified value when
    /// <see cref="System.DBNull" /> is returned from the data source, and adding the binding to the collection.
    /// </summary>
    /// <returns>The newly created
    /// <see cref="System.Windows.Forms.Binding" />
    /// </returns>
    /// <param name="propertyName">The name of the control property to bind.
    /// </param>
    /// <param name="dataSource">An
    /// <see cref="System.Object" /> representing the data source.
    /// </param>
    /// <param name="dataMember">The property or list to bind to.
    /// </param>
    /// <param name="formattingEnabled">true to format the displayed data; otherwise, false.
    /// </param>
    /// <param name="updateMode">One of the
    /// <see cref="System.Windows.Forms.DataSourceUpdateMode" /> values.
    /// </param>
    /// <param name="nullValue">The
    /// <see cref="System.Object" /> to be applied to the bound control property if the data source value is
    /// <see cref="System.DBNull" />
    /// </param>
    /// <param name="formatString">One or more format specifier characters that indicate how a value is to be displayed.
    /// </param>
    /// <exception cref="System.ArgumentException">The property given by
    /// <paramref name="propertyName" /> does not exist on the control or is read-only.-or-The specified data member does not exist on the data source.-or-The data source, data member, or control property specified are associated with another binding in the collection.
    /// </exception>
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
    /// Creates a binding that binds the specified control property to the specified data member of the specified data source, optionally enabling formatting with the specified format string, propagating values to the data source based on the specified update setting, setting the property to the specified value when
    /// <see cref="System.DBNull" /> is returned from the data source, setting the specified format provider, and adding the binding to the collection.
    /// </summary>
    /// <returns>The newly created
    /// <see cref="System.Windows.Forms.Binding" />
    /// </returns>
    /// <param name="propertyName">The name of the control property to bind.
    /// </param>
    /// <param name="dataSource">An
    /// <see cref="System.Object" /> representing the data source.
    /// </param>
    /// <param name="dataMember">The property or list to bind to.
    /// </param>
    /// <param name="formattingEnabled">true to format the displayed data; otherwise, false.
    /// </param>
    /// <param name="updateMode">One of the
    /// <see cref="System.Windows.Forms.DataSourceUpdateMode" /> values.
    /// </param>
    /// <param name="nullValue">The
    /// <see cref="System.Object" /> to be applied to the bound control property if the data source value is
    /// <see cref="System.DBNull" />
    /// </param>
    /// <param name="formatString">One or more format specifier characters that indicate how a value is to be displayed
    /// </param>
    /// <param name="formatInfo">An implementation of
    /// <see cref="System.IFormatProvider" /> to override default formatting behavior.
    /// </param>
    /// <exception cref="System.ArgumentException">The property given by
    /// <paramref name="propertyName" /> does not exist on the control or is read-only.-or-The specified data member does not exist on the data source.-or-The data source, data member, or control property specified are associated with another binding in the collection.
    /// </exception>
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
    /// Clears the collection of any bindings.
    /// Fires the CollectionChangedEvent.
    /// </summary>
    public new void Clear() => base.Clear();

    /// <summary>
    /// Deletes the specified
    /// <see cref="System.Windows.Forms.Binding" /> from the collection.
    /// </summary>
    /// <param name="binding">The
    /// <see cref="System.Windows.Forms.Binding" /> to remove.
    /// </param>
    /// <exception cref="System.NullReferenceException">The
    /// <paramref name="binding" /> is null.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    public new void Remove(Binding binding) => base.Remove(binding);

    /// <summary>
    /// Deletes the
    /// <see cref="System.Windows.Forms.Binding" /> at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the item to remove.
    /// </param>
    /// <exception cref="System.ArgumentOutOfRangeException">The
    /// <paramref name="index" /> value is less than 0, or it is greater than the number of bindings in the collection.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    public new void RemoveAt(int index) => base.RemoveAt(index);

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
    /// Creates the binding and adds it to the collection. An InvalidBindingException is
    /// thrown if this binding can't be constructed. An exception is thrown if a binding to
    /// the same target and Property as an existing binding or if the binding's column isn't
    /// a valid column given this DataSource.Table's schema.
    /// Fires the CollectionChangedEvent.
    /// </summary>
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
    /// Clears the bindings in the collection.
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
    /// Removes the specified binding from the collection.
    /// </summary>
    /// <param name="dataBinding">The
    /// <see cref="System.Windows.Forms.Binding" /> to remove from the collection.
    /// </param>
    /// <exception cref="System.ArgumentException">The binding belongs to another
    /// <see cref="System.Windows.Forms.ControlBindingsCollection" />
    /// </exception>
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
