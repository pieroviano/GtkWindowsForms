// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms;
/// <summary>
/// Manages all
/// <see cref="System.Windows.Forms.Binding" />
/// objects that are bound to the same data source and data member. This class is abstract.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public abstract class BindingManagerBase
{

    private BindingsCollection? _bindings;
    private bool _pullingData;

    protected EventHandler? OnCurrentChangedHandler; // Don't rename (breaking change)

    protected EventHandler? OnCurrentItemChangedHandler; // Don't rename (breaking change)

    protected EventHandler? OnPositionChangedHandler; // Don't rename (breaking change)

    // Hook BindingComplete events on all owned Binding objects, and propagate those events through our own BindingComplete event
    private BindingCompleteEventHandler? _bindingCompleteEventHandler;

    // same deal about the new currentItemChanged event
    private protected EventHandler? OnCurrentItemChangedValueHandler;

    // Event handler for the DataError event
    private BindingManagerDataErrorEventHandler? _dataError;
    /// <summary>
    /// Gets the collection of bindings being managed.
    /// </summary>
    /// <returns>A
    /// <see cref="System.Windows.Forms.BindingsCollection" /> that contains the
    /// <see cref="System.Windows.Forms.Binding" /> objects managed by this
    /// <see cref="System.Windows.Forms.BindingManagerBase" />
    /// </returns>
    /// <filterpriority>1
    /// </filterpriority>
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
    /// Raises the
    /// <see cref="System.Windows.Forms.BindingManagerBase.BindingComplete" /> event.
    /// </summary>
    /// <param name="args">A
    /// <see cref="System.Windows.Forms.BindingCompleteEventArgs" />  that contains the event data.
    /// </param>
    protected internal void OnBindingComplete(BindingCompleteEventArgs e)
    {
        _bindingCompleteEventHandler?.Invoke(this, e);
    }
    /// <summary>
    /// Raises the
    /// <see cref="System.Windows.Forms.BindingManagerBase.CurrentChanged" /> event.
    /// </summary>
    /// <param name="e">The
    /// <see cref="System.EventArgs" /> that contains the event data.
    /// </param>
    protected internal abstract void OnCurrentChanged(EventArgs e);
    /// <summary>
    /// Raises the
    /// <see cref="System.Windows.Forms.BindingManagerBase.CurrentItemChanged" /> event.
    /// </summary>
    /// <param name="e">The
    /// <see cref="System.EventArgs" /> that contains the event data.
    /// </param>
    protected internal abstract void OnCurrentItemChanged(EventArgs e);
    /// <summary>
    /// Raises the
    /// <see cref="System.Windows.Forms.BindingManagerBase.DataError" /> event.
    /// </summary>
    /// <param name="e">An
    /// <see cref="System.Exception" /> that caused the
    /// <see cref="System.Windows.Forms.BindingManagerBase.DataError" /> event to occur.
    /// </param>
    protected internal void OnDataError(Exception e)
    {
        _dataError?.Invoke(this, new BindingManagerDataErrorEventArgs(e));
    }
    /// <summary>
    /// When overridden in a derived class, gets the current object.
    /// </summary>
    /// <returns>An
    /// <see cref="System.Object" /> that represents the current object.
    /// </returns>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract object? Current { get; }

    private protected abstract void SetDataSource(object? source);
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="System.Windows.Forms.BindingManagerBase" /> class.
    /// </summary>
    public BindingManagerBase() { }

    internal BindingManagerBase(object? dataSource) => Init(dataSource);

    private void Init(object? dataSource)
    {
        SetDataSource(dataSource);
    }

    internal abstract Type? BindType { get; }

    internal abstract PropertyDescriptorCollection? GetItemProperties(PropertyDescriptor?[]? listAccessors);
    /// <summary>
    /// When overridden in a derived class, gets the collection of property descriptors for the binding.
    /// </summary>
    /// <returns>A
    /// <see cref="System.ComponentModel.PropertyDescriptorCollection" /> that represents the property descriptors for the binding.
    /// </returns>
    /// <filterpriority>1
    /// </filterpriority>
    public virtual PropertyDescriptorCollection? GetItemProperties() => GetItemProperties(listAccessors: null);
    /// <summary>
    /// Gets the collection of property descriptors for the binding using the specified
    /// <see cref="System.Collections.ArrayList" />
    /// </summary>
    /// <returns>A
    /// <see cref="System.ComponentModel.PropertyDescriptorCollection" /> that represents the property descriptors for the binding.
    /// </returns>
    /// <param name="dataSources">An
    /// <see cref="System.Collections.ArrayList" /> containing the data sources.
    /// </param>
    /// <param name="listAccessors">An
    /// <see cref="System.Collections.ArrayList" /> containing the table's bound properties.
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
    /// Gets the list of properties of the items managed by this
    /// <see cref="System.Windows.Forms.BindingManagerBase" />
    /// </summary>
    /// <returns>A
    /// <see cref="System.ComponentModel.PropertyDescriptorCollection" /> that represents the property descriptors for the binding.
    /// </returns>
    /// <param name="listType">The
    /// <see cref="System.Type" /> of the bound list.
    /// </param>
    /// <param name="offset">A counter used to recursively call the method.
    /// </param>
    /// <param name="dataSources">An
    /// <see cref="System.Collections.ArrayList" /> containing the data sources.
    /// </param>
    /// <param name="listAccessors">An
    /// <see cref="System.Collections.ArrayList" /> containing the table's bound properties.
    /// </param>
    protected virtual PropertyDescriptorCollection? GetItemProperties(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? listType,
            int offset,
            ArrayList dataSources,
        ArrayList listAccessors)
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

    public event BindingCompleteEventHandler? BindingComplete
    {
        add => _bindingCompleteEventHandler += value;
        remove => _bindingCompleteEventHandler -= value;
    }

    public event EventHandler? CurrentChanged
    {
        add => OnCurrentChangedHandler += value;
        remove => OnCurrentChangedHandler -= value;
    }

    public event EventHandler? CurrentItemChanged
    {
        add => OnCurrentItemChangedValueHandler += value;
        remove => OnCurrentItemChangedValueHandler -= value;
    }

    public event BindingManagerDataErrorEventHandler? DataError
    {
        add => _dataError += value;
        remove => _dataError -= value;
    }


    internal abstract string? GetListName();
    /// <summary>
    /// When overridden in a derived class, cancels the current edit.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract void CancelCurrentEdit();
    /// <summary>
    /// When overridden in a derived class, ends the current edit.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract void EndCurrentEdit();
    /// <summary>
    /// When overridden in a derived class, adds a new item to the underlying list.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract void AddNew();
    /// <summary>
    /// When overridden in a derived class, deletes the row at the specified index from the underlying list.
    /// </summary>
    /// <param name="index">The index of the row to delete.
    /// </param>
    /// <exception cref="System.IndexOutOfRangeException">There is no row at the specified
    /// <paramref name="index" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract void RemoveAt(int index);
    /// <summary>
    /// When overridden in a derived class, gets or sets the position in the underlying list that controls bound to this data source point to.
    /// </summary>
    /// <returns>A zero-based index that specifies a position in the underlying list.
    /// </returns>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract int Position { get; set; }

    public event EventHandler? PositionChanged
    {
        add => OnPositionChangedHandler += value;
        remove => OnPositionChangedHandler -= value;
    }
    /// <summary>
    /// When overridden in a derived class, updates the binding.
    /// </summary>
    protected abstract void UpdateIsBinding();
    /// <summary>
    /// When overridden in a derived class, gets the name of the list supplying the data for the binding.
    /// </summary>
    /// <returns>The name of the list supplying the data for the binding.
    /// </returns>
    /// <param name="listAccessors">An
    /// <see cref="System.Collections.ArrayList" /> containing the table's bound properties.
    /// </param>
    protected internal abstract string? GetListName(ArrayList? listAccessors);
    /// <summary>
    /// When overridden in a derived class, suspends data binding.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract void SuspendBinding();
    /// <summary>
    /// When overridden in a derived class, resumes data binding.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract void ResumeBinding();
    /// <summary>
    /// Pulls data from the data-bound control into the data source, returning no information.
    /// </summary>
    protected void PullData() => PullData(out _);

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
    /// Pushes data from the data source into the data-bound control, returning no information.
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

    internal abstract object? DataSource { get; }

    internal abstract bool IsBinding { get; }
    /// <summary>
    /// Gets a value indicating whether binding is suspended.
    /// </summary>
    /// <returns>true if binding is suspended; otherwise, false.
    /// </returns>
    public bool IsBindingSuspended => !IsBinding;
    /// <summary>
    /// When overridden in a derived class, gets the number of rows managed by the
    /// <see cref="System.Windows.Forms.BindingManagerBase" />
    /// </summary>
    /// <returns>The number of rows managed by the
    /// <see cref="System.Windows.Forms.BindingManagerBase" />
    /// </returns>
    /// <filterpriority>1
    /// </filterpriority>
    public abstract int Count { get; }

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

    private void Binding_BindingComplete(object? sender, BindingCompleteEventArgs args)
    {
        OnBindingComplete(args);
    }
}
