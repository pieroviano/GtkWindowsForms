using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a collection of
/// <see cref="T:System.Windows.Forms.Binding"/> objects for a control.
/// Provides add/remove/clear semantics and raises collection change events so
/// listeners (for example a
/// <see cref="T:System.Windows.Forms.BindingManagerBase"/>) can react to changes.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
[DefaultEvent("CollectionChanged")]
public class BindingsCollection : BaseCollection
{
    private readonly List<Binding> _list = new List<Binding>();

    private CollectionChangeEventHandler? _onCollectionChanged;

    private CollectionChangeEventHandler? _onCollectionChanging;

    /// <summary>
    /// Occurs after the collection has changed (Add/Remove/Refresh).
    /// </summary>
    public event CollectionChangeEventHandler? CollectionChanged { add => _onCollectionChanged += value; remove => _onCollectionChanged -= value; }

    /// <summary>
    /// Occurs before the collection changes. Handlers can inspect the change before it is applied.
    /// </summary>
    public event CollectionChangeEventHandler? CollectionChanging { add => _onCollectionChanging += value; remove => _onCollectionChanging -= value; }

    internal BindingsCollection()
    {
    }

    /// <summary>
    /// Gets the number of bindings in the collection.
    /// </summary>
    public override int Count => _list.Count;

    /// <summary>
    /// Gets an
    /// <see cref="T:System.Collections.ArrayList"/> adapter for the underlying list.
    /// This implements the abstract member from
    /// <see cref="T:System.Windows.Forms.BaseCollection"/>.
    /// </summary>
    protected override ArrayList List => ArrayList.Adapter(_list);

    /// <summary>
    /// Gets the
    /// <see cref="T:System.Windows.Forms.Binding"/> at the specified index.
    /// </summary>
    /// <param name = "index">
    /// Zero-based index of the binding to retrieve.
    /// </param>
    /// <returns>
    /// The
    /// <see cref="T:System.Windows.Forms.Binding"/> at the specified index.
    /// </returns>
    public Binding this[int index] => _list[index]!;

    /// <summary>
    /// Adds the specified binding to the collection and raises the appropriate changing/changed events.
    /// Intended for internal use by binding infrastructure.
    /// </summary>
    /// <param name = "binding">
    /// The binding to add.
    /// </param>
    protected internal void Add(Binding binding)
    {
        var eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Add, binding);
        OnCollectionChanging(eventArgs);
        AddCore(binding);
        OnCollectionChanged(eventArgs);
    }

    /// <summary>
    /// Clears the collection and raises Refresh-style changing/changed events.
    /// </summary>
    protected internal void Clear()
    {
        var eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
        OnCollectionChanging(eventArgs);
        ClearCore();
        OnCollectionChanged(eventArgs);
    }

    /// <summary>
    /// Removes the specified binding from the collection and raises the appropriate events.
    /// </summary>
    /// <param name = "binding">
    /// The binding to remove.
    /// </param>
    protected internal void Remove(Binding binding)
    {
        var eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Remove, binding);
        OnCollectionChanging(eventArgs);
        RemoveCore(binding);
        OnCollectionChanged(eventArgs);
    }

    /// <summary>
    /// Removes the binding at the specified index.
    /// </summary>
    /// <param name = "index">
    /// Index of the binding to remove.
    /// </param>
    protected internal void RemoveAt(int index) => Remove(this[index]);

    /// <summary>
    /// Returns true when the collection contains any bindings. Used for serialization helpers.
    /// </summary>
    protected internal bool ShouldSerializeMyAll() => Count > 0;

    /// <summary>
    /// Core virtual method that actually performs the add operation.  Derived classes can override.
    /// </summary>
    /// <param name = "dataBinding">
    /// The binding to add.
    /// </param>
    protected virtual void AddCore(Binding dataBinding)
    {
        _list.Add(dataBinding);
    }

    /// <summary>
    /// Core virtual method that actually clears the underlying storage. Derived classes can override.
    /// </summary>
    protected virtual void ClearCore() => _list.Clear();

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.BindingsCollection.CollectionChanged"/> event.
    /// </summary>
    /// <param name = "ccevent">
    /// Event arguments describing the change.
    /// </param>
    protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
    {
        _onCollectionChanged?.Invoke(this, ccevent);
    }

    /// <summary>
    /// Raises the
    /// <see cref="T:System.Windows.Forms.BindingsCollection.CollectionChanging"/> event.
    /// </summary>
    /// <param name = "e">
    /// Event arguments describing the impending change.
    /// </param>
    protected virtual void OnCollectionChanging(CollectionChangeEventArgs e)
    {
        _onCollectionChanging?.Invoke(this, e);
    }

    /// <summary>
    /// Core virtual method that actually performs the remove operation.  Derived classes can override.
    /// </summary>
    /// <param name = "dataBinding">
    /// The binding to remove.
    /// </param>
    protected virtual void RemoveCore(Binding dataBinding) => _list.Remove(dataBinding);

}