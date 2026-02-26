using Gtk;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of
    /// <see cref="System.Windows.Forms.Binding" />
    /// objects for a control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DefaultEvent("CollectionChanged")]
    public class BindingsCollection : BaseCollection
    {

        private List<Binding> _list = new List<Binding>();

        private CollectionChangeEventHandler? _onCollectionChanging;
        private CollectionChangeEventHandler? _onCollectionChanged;

        internal BindingsCollection()
        {
        }
        /// <summary>
        /// Gets the total number of bindings in the collection.
        /// </summary>
        /// <returns>The total number of bindings in the collection.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override int Count => _list.Count;
        /// <summary>
        /// Gets the bindings in the collection as an object.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Collections.ArrayList" /> containing all of the collection members.
        /// </returns>
        protected override ArrayList List => ArrayList.Adapter(_list);

        public Binding this[int index] => _list[index]!;
        /// <summary>
        /// Adds the specified binding to the collection.
        /// </summary>
        /// <param name="binding">The
        /// <see cref="System.Windows.Forms.Binding" /> to add to the collection.
        /// </param>
        protected internal void Add(Binding binding)
        {
            CollectionChangeEventArgs eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Add, binding);
            OnCollectionChanging(eventArgs);
            AddCore(binding);
            OnCollectionChanged(eventArgs);
        }
        /// <summary>
        /// Adds a
        /// <see cref="System.Windows.Forms.Binding" /> to the collection.
        /// </summary>
        /// <param name="dataBinding">The
        /// <see cref="System.Windows.Forms.Binding" /> to add to the collection.
        /// </param>
        /// <exception cref="System.ArgumentNullException">The
        /// <paramref name="dataBinding" /> argument was null.
        /// </exception>
        protected virtual void AddCore(Binding dataBinding)
        {
            _list.Add(dataBinding);
        }
        public event CollectionChangeEventHandler? CollectionChanging
        {
            add => _onCollectionChanging += value;
            remove => _onCollectionChanging -= value;
        }

        public event CollectionChangeEventHandler? CollectionChanged
        {
            add => _onCollectionChanged += value;
            remove => _onCollectionChanged -= value;
        }
        /// <summary>
        /// Clears the collection of binding objects.
        /// </summary>
        protected internal void Clear()
        {
            CollectionChangeEventArgs eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
            OnCollectionChanging(eventArgs);
            ClearCore();
            OnCollectionChanged(eventArgs);
        }
        /// <summary>
        /// Clears the collection of any members.
        /// </summary>
        protected virtual void ClearCore() => _list.Clear();
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.BindingsCollection.CollectionChanging" /> event.
        /// </summary>
        /// <param name="e">A
        /// <see cref="System.ComponentModel.CollectionChangeEventArgs" /> that contains event data.
        /// </param>
        protected virtual void OnCollectionChanging(CollectionChangeEventArgs e)
        {
            _onCollectionChanging?.Invoke(this, e);
        }
        /// <summary>
        /// Raises the
        /// <see cref="System.Windows.Forms.BindingsCollection.CollectionChanged" /> event.
        /// </summary>
        /// <param name="ccevent">A
        /// <see cref="System.ComponentModel.CollectionChangeEventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
        {
            _onCollectionChanged?.Invoke(this, ccevent);
        }
        /// <summary>
        /// Deletes the specified binding from the collection.
        /// </summary>
        /// <param name="binding">The Binding to remove from the collection.
        /// </param>
        protected internal void Remove(Binding binding)
        {
            CollectionChangeEventArgs eventArgs = new CollectionChangeEventArgs(CollectionChangeAction.Remove, binding);
            OnCollectionChanging(eventArgs);
            RemoveCore(binding);
            OnCollectionChanged(eventArgs);
        }
        /// <summary>
        /// Deletes the binding from the collection at the specified index.
        /// </summary>
        /// <param name="index">The index of the
        /// <see cref="System.Windows.Forms.Binding" /> to remove.
        /// </param>
        protected internal void RemoveAt(int index) => Remove(this[index]);
        /// <summary>
        /// Removes the specified
        /// <see cref="System.Windows.Forms.Binding" /> from the collection.
        /// </summary>
        /// <param name="dataBinding">The
        /// <see cref="System.Windows.Forms.Binding" /> to remove.
        /// </param>
        protected virtual void RemoveCore(Binding dataBinding) => _list.Remove(dataBinding);
        /// <summary>
        /// Gets a value that indicates whether the collection should be serialized.
        /// </summary>
        /// <returns>true if the collection count is greater than zero; otherwise, false.
        /// </returns>
        protected internal bool ShouldSerializeMyAll() => Count > 0;
    }
}
