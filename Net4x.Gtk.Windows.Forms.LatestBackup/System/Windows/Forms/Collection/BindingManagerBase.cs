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
    protected EventHandler? OnCurrentChangedHandler; // Don't rename (breaking change)

    protected EventHandler? OnCurrentItemChangedHandler; // Don't rename (breaking change)

    // same deal about the new currentItemChanged event
    private protected EventHandler? OnCurrentItemChangedValueHandler;

    protected EventHandler? OnPositionChangedHandler; // Don't rename (breaking change)

    // Hook BindingComplete events on all owned Binding objects, and propagate those events through our own BindingComplete event
    private BindingCompleteEventHandler? _bindingCompleteEventHandler;

    private BindingsCollection? _bindings;

    // Event handler for the DataError event
    private BindingManagerDataErrorEventHandler? _dataError;

    private bool _pullingData;

    public event BindingCompleteEventHandler? BindingComplete { add => _bindingCompleteEventHandler += value; remove => _bindingCompleteEventHandler -= value; }

    public event EventHandler? CurrentChanged { add => OnCurrentChangedHandler += value; remove => OnCurrentChangedHandler -= value; }

    public event EventHandler? CurrentItemChanged { add => OnCurrentItemChangedValueHandler += value; remove => OnCurrentItemChangedValueHandler -= value; }

    public event BindingManagerDataErrorEventHandler? DataError { add => _dataError += value; remove => _dataError -= value; }

    public event EventHandler? PositionChanged { add => OnPositionChangedHandler += value; remove => OnPositionChangedHandler -= value; }

    public BindingManagerBase()
    {
    }

    internal BindingManagerBase(object? dataSource) => Init(dataSource);

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

    public abstract int Count { get; }

    public abstract object? Current { get; }

    public bool IsBindingSuspended => !IsBinding;

    public abstract int Position { get; set; }

    internal abstract Type? BindType { get; }

    internal abstract object? DataSource { get; }

    internal abstract bool IsBinding { get; }

    public abstract void AddNew();

    public abstract void CancelCurrentEdit();

    public abstract void EndCurrentEdit();

    public virtual PropertyDescriptorCollection? GetItemProperties() => GetItemProperties(listAccessors: null);

    public abstract void RemoveAt(int index);

    public abstract void ResumeBinding();

    public abstract void SuspendBinding();

    internal abstract PropertyDescriptorCollection? GetItemProperties(PropertyDescriptor?[]? listAccessors);

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

    internal abstract string? GetListName();

    protected internal abstract string? GetListName(ArrayList? listAccessors);

    protected internal void OnBindingComplete(BindingCompleteEventArgs e)
    {
        _bindingCompleteEventHandler?.Invoke(this, e);
    }

    protected internal abstract void OnCurrentChanged(EventArgs e);

    protected internal abstract void OnCurrentItemChanged(EventArgs e);

    protected internal void OnDataError(Exception e)
    {
        _dataError?.Invoke(this, new BindingManagerDataErrorEventArgs(e));
    }

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

    protected void PullData() => PullData(out _);

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
    ///
    /// For the Add and Remove cases, we hook the collection 'changed' event, and add or remove handler for
    /// specific binding.
    ///
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
