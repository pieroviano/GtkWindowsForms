// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms;
/// <summary>
/// Manages all
/// <see cref="T:System.Windows.Forms.Binding"/>
/// objects that are bound to the same data source and data member. This class is abstract.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public abstract class BindingManagerBase
{
    /// <summary>
    /// The on current changed handler
    /// </summary>
    protected EventHandler? OnCurrentChangedHandler; // Don't rename (breaking change)

    /// <summary>
    /// The on current item changed handler
    /// </summary>
    protected EventHandler? OnCurrentItemChangedHandler; // Don't rename (breaking change)

    // same deal about the new currentItemChanged event
    private protected EventHandler? OnCurrentItemChangedValueHandler;

    /// <summary>
    /// The on position changed handler
    /// </summary>
    protected EventHandler? OnPositionChangedHandler; // Don't rename (breaking change)

    // Hook BindingComplete events on all owned Binding objects, and propagate those events through our own BindingComplete event
    private BindingCompleteEventHandler? _bindingCompleteEventHandler;

    private BindingsCollection? _bindings;

    // Event handler for the DataError event
    private BindingManagerDataErrorEventHandler? _dataError;

    private bool _pullingData;

    /// <summary>
    /// Occurs when a binding operation has completed. Individual binding objects raise BindingComplete,
    /// and this event aggregates those notifications for bindings managed by this manager.
    /// </summary>
    public event BindingCompleteEventHandler? BindingComplete { add => _bindingCompleteEventHandler += value; remove => _bindingCompleteEventHandler -= value; }

    /// <summary>
    /// Occurs when the current item has changed.
    /// </summary>
    public event EventHandler? CurrentChanged { add => OnCurrentChangedHandler += value; remove => OnCurrentChangedHandler -= value; }

    /// <summary>
    /// Occurs after the current item has changed; provided for compatibility with existing patterns.
    /// </summary>
    public event EventHandler? CurrentItemChanged { add => OnCurrentItemChangedValueHandler += value; remove => OnCurrentItemChangedValueHandler -= value; }

    /// <summary>
    /// Occurs when a data error is detected while synchronizing values between source and bound controls.
    /// </summary>
    public event BindingManagerDataErrorEventHandler? DataError { add => _dataError += value; remove => _dataError -= value; }

    /// <summary>
    /// Occurs when the position within the managed list changes.
    /// </summary>
    public event EventHandler? PositionChanged { add => OnPositionChangedHandler += value; remove => OnPositionChangedHandler -= value; }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BindingManagerBase"/> class.
    /// </summary>
    public BindingManagerBase()
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.BindingManagerBase"/> class.
    /// </summary>
    internal BindingManagerBase(object? dataSource) => Init(dataSource);

    /// <summary>
    /// Gets the collection of
    /// <see cref="T:System.Windows.Forms.Binding"/> objects managed by this instance.
    /// The collection is lazily created on first access and hooks collection change events
    /// to allow propagation of BindingComplete events from individual bindings.
    /// </summary>
    public BindingsCollection Bindings
    {
        get
        {
            if (_bindings is null)
            {
                _bindings = new ListManagerBindingsCollection(this);
                // Hook collection change events on collection, so we can hook or unhook the BindingComplete events on individual bindings
                _bindings.CollectionChanging += OnBindingsCollectionChanging;
                _bindings.CollectionChanged += OnBindingsCollectionChanged;
            }

            return _bindings;
        }
    }

    /// <summary>
    /// Gets the number of items in the underlying list.
    /// </summary>
    public abstract int Count { get; }

    /// <summary>
    /// Gets the current item in the underlying list.
    /// </summary>
    public abstract object? Current { get; }

    /// <summary>
    /// Gets a value indicating whether binding operations are suspended.
    /// True when binding is suspended, false otherwise.
    /// </summary>
    public bool IsBindingSuspended => !IsBinding;

    /// <summary>
    /// Gets or sets the current position (zero-based index) within the underlying list.
    /// </summary>
    public abstract int Position { get; set; }

    internal abstract Type? BindType { get; }

    internal abstract object? DataSource { get; }

    /// <summary>
    /// Gets a value indicating whether binding operations are suspended.
    /// True when binding is suspended, false otherwise.
    /// </summary>
    internal abstract bool IsBinding { get; }

    /// <summary>
    /// Adds a new item to the underlying list. Implementations should position the manager
    /// on the new item if appropriate.
    /// </summary>
    public abstract void AddNew();

    /// <summary>
    /// Cancels the current edit operation and restores the previous value.
    /// </summary>
    public abstract void CancelCurrentEdit();

    /// <summary>
    /// Commits the current edit operation.
    /// </summary>
    public abstract void EndCurrentEdit();

    /// <summary>
    /// Returns a collection of properties for items in the managed list.
    /// Calls through to
    /// <see cref="T:System.Windows.Forms.BindingManagerBase.GetItemProperties(System.ComponentModel.PropertyDescriptor[])"/>.
    /// </summary>
    public virtual PropertyDescriptorCollection? GetItemProperties() => GetItemProperties(listAccessors: null);

    /// <summary>
    /// Removes the item at the specified index from the underlying list.
    /// </summary>
    /// <param name = "index">
    /// Zero-based index of the item to remove.
    /// </param>
    public abstract void RemoveAt(int index);

    /// <summary>
    /// Resumes data binding after a previous
    /// <see cref="T:System.Windows.Forms.BindingManagerBase.SuspendBinding"/> call.
    /// </summary>
    public abstract void ResumeBinding();

    /// <summary>
    /// Suspends data binding operations. While suspended, push/pull of data is suppressed.
    /// </summary>
    public abstract void SuspendBinding();

    /// <summary>
    /// Returns a collection of properties for items in the managed list.
    /// Calls through to
    /// <see cref="T:System.Windows.Forms.BindingManagerBase.GetItemProperties(System.ComponentModel.PropertyDescriptor[])"/>.
    /// </summary>
    internal abstract PropertyDescriptorCollection? GetItemProperties(PropertyDescriptor?[]? listAccessors);

    /// <summary>
    /// Returns a collection of item properties for the specified data sources and accessors.
    /// This overload is used by complex related-currency-manager scenarios.
    /// </summary>
    /// <param name = "dataSources">
    /// ArrayList of data source objects participating in the relation.
    /// </param>
    /// <param name = "listAccessors">
    /// ArrayList of accessors that describe navigation into related lists.
    /// </param>
    protected internal virtual PropertyDescriptorCollection? GetItemProperties(ArrayList dataSources, ArrayList listAccessors)
    {
        IList? list = null;
        if (this is CurrencyManager currencyManager)
        {
            list = currencyManager.List;
        }

        if (list is ITypedList typedList)
        {
            var properties = new PropertyDescriptor[listAccessors.Count];
            listAccessors.CopyTo(properties, 0);
            return typedList.GetItemProperties(properties);
        }

        return GetItemProperties(BindType, 0, dataSources, listAccessors);
    }

    /// <summary>
    /// Gets the name of the list.
    /// </summary>
    /// <param name = "listAccessors">
    /// The list accessors.
    /// </param>
    /// <returns>
    /// </returns>
    internal abstract string? GetListName();

    /// <summary>
    /// Gets the name of the list.
    /// </summary>
    /// <param name = "listAccessors">
    /// The list accessors.
    /// </param>
    /// <returns>
    /// </returns>
    protected internal abstract string? GetListName(ArrayList? listAccessors);

    /// <summary>
    /// Raises the aggregated BindingComplete event for this manager.
    /// </summary>
    /// <param name = "e">
    /// Event arguments describing the binding completion.
    /// </param>
    protected internal void OnBindingComplete(BindingCompleteEventArgs e)
    {
        _bindingCompleteEventHandler?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.BindingManagerBase.CurrentChanged"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    protected internal abstract void OnCurrentChanged(EventArgs e);

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.BindingManagerBase.CurrentItemChanged"/> event.
    /// </summary>
    /// <param name = "e">
    /// The
    /// <see cref="T:System.EventArgs"/> instance containing the event data.
    /// </param>
    protected internal abstract void OnCurrentItemChanged(EventArgs e);

    /// <summary>
    /// Raises the DataError event with the provided exception.
    /// </summary>
    /// <param name = "e">
    /// The exception that occurred.
    /// </param>
    protected internal void OnDataError(Exception e)
    {
        _dataError?.Invoke(this, new BindingManagerDataErrorEventArgs(e));
    }

    /// <summary>
    /// Convenience wrapper around PullData that discards the success output.
    /// </summary>
    internal void PullData(out bool success)
    {
        success = true;
        _pullingData = true;
        try
        {
            UpdateIsBinding();
            var numLinks = Bindings.Count;
            for (var i = 0; i < numLinks; i++)
            {
                if (Bindings[i].PullData())
                {
                    success = false;
                }
            }
        }
        finally
        {
            _pullingData = false;
        }
    }

    /// <summary>
    /// Gets the item properties.
    /// </summary>
    /// <param name = "listType">
    /// Type of the list.
    /// </param>
    /// <param name = "offset">
    /// The offset.
    /// </param>
    /// <param name = "dataSources">
    /// The data sources.
    /// </param>
    /// <param name = "listAccessors">
    /// The list accessors.
    /// </param>
    /// <returns>
    /// </returns>
    protected virtual PropertyDescriptorCollection? GetItemProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? listType, int offset, ArrayList dataSources, ArrayList listAccessors)
    {
        if (listAccessors.Count < offset)
        {
            return null;
        }

        if (listAccessors.Count == offset)
        {
            if (!typeof(IList).IsAssignableFrom(listType))
            {
                return TypeDescriptor.GetProperties(listType);
            }

            if (listType != null)
            {
                foreach (var property in listType.GetProperties())
                {
                    if (property.Name == "Item" && property.PropertyType != typeof(object))
                    {
                        return TypeDescriptor.GetProperties(property.PropertyType, [new BrowsableAttribute(true)]);
                    }
                }
            }

            // return the properties on the type of the first element in the list
            if (dataSources[offset - 1] is IList { Count: > 0 } list)
            {
                return TypeDescriptor.GetProperties(list[0]!);
            }

            return null;
        }

        if (typeof(IList).IsAssignableFrom(listType))
        {
            PropertyDescriptorCollection? itemProps = null;
            if (listType != null)
            {
                foreach (var property in listType.GetProperties())
                {
                    if (property.Name == "Item" && property.PropertyType != typeof(object))
                    {
                        // get all the properties that are not marked as Browsable(false)
                        itemProps = TypeDescriptor.GetProperties(property.PropertyType, [new BrowsableAttribute(true)]);
                    }
                }
            }

            if (itemProps is null)
            {
                // Use the properties on the type of the first element in the list
                // if offset == 0, then this means that the first dataSource did not have a strongly typed Item property.
                // the dataSources are added only for relatedCurrencyManagers, so in this particular case
                // we need to use the dataSource in the currencyManager.
                IList? list;
                if (offset == 0)
                {
                    list = DataSource as IList;
                }
                else
                {
                    list = dataSources[offset - 1] as IList;
                }

                if (list is not null && list.Count > 0)
                {
                    itemProps = TypeDescriptor.GetProperties(list[0]!);
                }
            }

            if (itemProps is not null)
            {
                for (var j = 0; j < itemProps.Count; j++)
                {
                    if (itemProps[j].Equals(listAccessors[offset]))
                    {
                        return GetItemProperties(itemProps[j].PropertyType, offset + 1, dataSources, listAccessors);
                    }
                }
            }
        }
        else
        {
            if (listType != null)
            {
                foreach (var property in listType.GetProperties())
                {
                    if (property.Name.Equals(((PropertyDescriptor)listAccessors[offset]!).Name))
                    {
                        return GetItemProperties(property.PropertyType, offset + 1, dataSources, listAccessors);
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Convenience wrapper around PullData that discards the success output.
    /// </summary>
    protected void PullData() => PullData(out _);

    /// <summary>
    /// Pushes pending changes from bound controls back to their data source.
    /// </summary>
    protected void PushData()
    {
        if (_pullingData)
        {
            return;
        }

        UpdateIsBinding();
        var numLinks = Bindings.Count;
        for (var i = 0; i < numLinks; i++)
        {
            Bindings[i].PushData();
        }
    }

    private protected abstract void SetDataSource(object? source);

    /// <summary>
    /// Updates the is binding.
    /// </summary>
    protected abstract void UpdateIsBinding();

    private void Binding_BindingComplete(object? sender, BindingCompleteEventArgs args)
    {
        OnBindingComplete(args);
    }

    private void Init(object? dataSource)
    {
        SetDataSource(dataSource);
    }

    /// <summary>
    /// BindingComplete events on individual Bindings are propagated up through the BindingComplete event on
    /// the owning BindingManagerBase. To do this, we have to track changes to the bindings collection, adding
    /// or removing handlers on items in the collection as appropriate.
    /// For the Add and Remove cases, we hook the collection 'changed' event, and add or remove handler for
    /// specific binding.
    /// For the Refresh case, we hook both the 'changing' and 'changed' events, removing handlers for all
    /// items that were in the collection before the change, then adding handlers for whatever items are
    /// in the collection after the change.
    /// </summary>
    private void OnBindingsCollectionChanged(object? sender, CollectionChangeEventArgs e)
    {
        if (e.Element is not Binding binding)
        {
            return;
        }

        switch (e.Action)
        {
            case CollectionChangeAction.Add:
                binding.BindingComplete += Binding_BindingComplete;
                break;
            case CollectionChangeAction.Remove:
                binding.BindingComplete -= Binding_BindingComplete;
                break;
            case CollectionChangeAction.Refresh:
                foreach (Binding bi in Bindings)
                {
                    bi.BindingComplete += Binding_BindingComplete;
                }

                break;
        }
    }

    private void OnBindingsCollectionChanging(object? sender, CollectionChangeEventArgs e)
    {
        if (e.Action != CollectionChangeAction.Refresh)
        {
            return;
        }

        foreach (Binding bi in Bindings)
        {
            bi.BindingComplete -= Binding_BindingComplete;
        }
    }

}