using System.Collections;
using System.ComponentModel;
using System.Windows.Forms.Layout;
using Gtk;
using Container = Gtk.Widget;

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    /// Collection that stores and manages child controls for a parent
    /// <see cref="T:System.Windows.Forms.Control" />.
    /// Implements layout-aware collection behavior and provides methods for adding, removing and locating child controls.
    /// </summary>
    public partial class ControlCollection : ArrangedElementCollection, IList, ICloneable
    {

        readonly Type? _itemType;

        readonly Control _owner;

        readonly Container? _ownerControl;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.Control.ControlCollection" /> class for the specified owner control.
        /// The collection hooks GTK container events required to manage child layout and mapping.
        /// </summary>
        /// <param name="owner">
        /// The owning
        /// <see cref="T:System.Windows.Forms.Control" />.
        /// </param>
        public ControlCollection(Control owner)
        {
            _ownerControl = owner.GtkControl as Gtk.Widget;
            _owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.Control.ControlCollection" /> class for the specified owner and GTK container.
        /// </summary>
        /// <param name="owner">
        /// The owning
        /// <see cref="T:System.Windows.Forms.Control" />.
        /// </param>
        /// <param name="ownerContainer">
        /// The underlying GTK container used to host children.
        /// </param>
        public ControlCollection(Control owner, Container? ownerContainer)
        {
            _ownerControl = ownerContainer;
            _owner = owner;
            if (_ownerControl != null)
            {
                _ownerControl.Mapped += OwnerContainer_Mapped;
        // Gtk 4: this per-control raw-event handler used a removed *EventArgs type; control-specific
        // input is re-wired via event controllers / native signals and verified at runtime (migration #11).
            }
        }

        /// <summary>
        /// Gets the owning
        /// <see cref="T:System.Windows.Forms.Control" /> for this collection.
        /// </summary>
        public Control Owner { get => _owner; }

        /// <summary>
        /// Gets the child control at the specified index.
        /// </summary>
        /// <param name="index">
        /// Zero-based index of the child.
        /// </param>
        public new virtual Control this[int index]
        {
            get
            {
                var control = (Control)InnerList[index]!;

                return control;
            }
        }

        /// <summary>
        /// Gets the child control with the specified key (name), or null if not found.
        /// </summary>
        /// <param name="key">
        /// The name/key of the control to find.
        /// </param>
        public virtual Control? this[string? key]
        {
            get
            {
                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }

                var index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    return this[index];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Adds a GTK widget to the owner control using native integration logic.
        /// </summary>
        /// <param name="value">
        /// GTK widget to add.
        /// </param>
        public virtual void Add(Gtk.Widget value)
        {
            NativeAdd(value);
        }

        /// <summary>
        /// Adds a control of the specified type to the collection. The current implementation forwards to
        /// <see cref="T:System.Windows.Forms.Control.ControlCollection.Add(System.Windows.Forms.Control)" />.
        /// </summary>
        /// <param name="itemType">
        /// Type of the item to add (ignored).
        /// </param>
        /// <param name="item">
        /// The control to add.
        /// </param>
        public virtual void Add(Type itemType, Control item)
        {
            // override handling
            this.Add(item);
        }

        /// <summary>
        /// Adds a
        /// <see cref="T:System.Windows.Forms.Control" /> to the collection and to the underlying GTK container.
        /// </summary>
        /// <param name="value">
        /// Control to add.
        /// </param>
        public virtual void Add(Control? value)
        {
            if (value is null)
            {
                return;
            }

            // Adding the same control twice used to duplicate it, and adding a control that already had a
            // parent left it in both collections. WinForms re-parents instead.
            if (InnerList.Contains(value))
            {
                return;
            }

            var previousParent = value.Parent;
            if (previousParent != null && !ReferenceEquals(previousParent, _owner))
            {
                previousParent.Controls?.Remove(value);
            }

            NativeAdd(value);
            InnerList.Add(value);
        }

        /// <summary>
        /// Adds an array of controls to the collection.
        /// </summary>
        /// <param name="controls">
        /// Controls to add.
        /// </param>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual void AddRange(params Control[] controls)
        {
            if (controls == null)
            {
                throw new ArgumentNullException(nameof(controls));
            }
            if (controls.Length == 0)
            {
                return;
            }
            foreach (var item in controls)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Adds a GTK widget and registers the corresponding
        /// <see cref="T:System.Windows.Forms.Control" /> as a child of the owner.
        /// </summary>
        /// <param name="item">
        /// GTK widget added.
        /// </param>
        /// <param name="control">
        /// Associated
        /// <see cref="T:System.Windows.Forms.Control" /> instance.
        /// </param>
        public void AddWidget(Gtk.Widget item, Control? control)
        {
            control?.Parent = _owner;
            InnerList.Add(new ArrangedElementWidget(item));
        }

        /// <summary>
        /// Removes all child widgets from the underlying GTK container and clears the internal list.
        /// </summary>
        public virtual void Clear()
        {
            if (_ownerControl != null)
            {
                foreach (var wid in _ownerControl.GetChildrenArray())
                    _ownerControl.RemoveChild(wid);
            }

            InnerList.Clear();
        }

        /// <summary>
        /// Creates a shallow clone of this collection containing the same arranged elements.
        /// </summary>
        /// <returns>
        /// A cloned
        /// <see cref="T:System.Windows.Forms.Control.ControlCollection" /> instance.
        /// </returns>
        public object Clone()
        {
            var ccOther = new ControlCollection(_owner, _ownerControl);
            ccOther.InnerList.AddRange(InnerList);

            return ccOther;
        }

        /// <summary>
        /// Determines whether the specified control is in the collection.
        /// </summary>
        /// <param name="control">
        /// Control to test.
        /// </param>
        /// <returns>
        /// True if found; otherwise false.
        /// </returns>
        public bool Contains(Control? control)
        {
            var innerList = ((IList)InnerList);
            if(innerList.Cast<object>().Any(o =>
            {
                var arrangedElementWidget = (o as System.Windows.Forms.Control.ArrangedElementWidget);
                var flowBoxChild = (arrangedElementWidget?.GetWidget as FlowBoxChild);
                return arrangedElementWidget != null && flowBoxChild != null && flowBoxChild.Child == control?.Widget;
            })) return true;
            return innerList.Contains(control);
        }

        /// <summary>
        /// Determines whether a control with the given key (name) exists in the collection.
        /// </summary>
        /// <param name="key">
        /// Key/name to search for.
        /// </param>
        /// <returns>
        /// True if a control with the key exists; otherwise false.
        /// </returns>
        public virtual bool ContainsKey(string? key)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            for (int i = 0; i < InnerList.Count; i++)
            {
                var o = InnerList[i];
                if (o is Control con)
                {
                    if (string.Equals(con.Name, key, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
                else if (o is ArrangedElementWidget widget)
                {
                    if (string.Equals(widget.GetWidget?.Name, key, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }

            return false;
        }

        public Control?[] Find(string key, bool searchAllChildren)
        {
            if (string.IsNullOrEmpty(key))
                return Array.Empty<Control?>();

            var results = new List<Control>();

            // Iterate explicitly to avoid depending on InnerList helper methods
            for (int i = 0; i < InnerList.Count; i++)
            {
                var o = InnerList[i];
                if (o is Control con)
                {
                    if (string.Equals(con.Name, key, StringComparison.OrdinalIgnoreCase))
                        results.Add(con);
                }
                else if (o is ArrangedElementWidget widget)
                {
                    var widgetName = widget.GetWidget?.Name;
                    if (string.Equals(widgetName, key, StringComparison.OrdinalIgnoreCase))
                    {
                        // If a Control instance is associated with this widget, return that Control.
                        for (int j = 0; j < InnerList.Count; j++)
                        {
                            if (InnerList[j] is Control candidate && candidate.Widget == widget.GetWidget)
                            {
                                results.Add(candidate);
                                break;
                            }
                        }
                    }
                }
            }

            // searchAllChildren was ignored, so a recursive Find never looked past the direct children.
            if (searchAllChildren)
            {
                for (int i = 0; i < InnerList.Count; i++)
                {
                    if (InnerList[i] is Control child && child.Controls != null)
                    {
                        foreach (var found in child.Controls.Find(key, true))
                        {
                            if (found != null)
                            {
                                results.Add(found);
                            }
                        }
                    }
                }
            }

            return results.ToArray();
        }

        public virtual int IndexOfKey(string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }

            for (int i = 0; i < InnerList.Count; i++)
            {
                var o = InnerList[i];
                if (o is Control con)
                {
                    if (string.Equals(con.Name, key, StringComparison.OrdinalIgnoreCase))
                        return i;
                }
                else if (o is ArrangedElementWidget widget)
                {
                    if (string.Equals(widget.GetWidget?.Name, key, StringComparison.OrdinalIgnoreCase))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes the specified child control from the collection and underlying GTK container.
        /// </summary>
        /// <param name="value">
        /// Control to remove.
        /// </param>
        public virtual void Remove(Control? value)
        {
            if (value is null)
            {
                return;
            }
            InnerList.Remove(value);
            if (_ownerControl?.GetChildrenArray().Any(o => o.Equals(value.Widget)) == true)
                _ownerControl.RemoveChild(value.Widget);
        }

        /// <summary>
        /// Removes the child at the specified index.
        /// </summary>
        /// <param name="index">
        /// Index to remove.
        /// </param>
        public void RemoveAt(int index)
        {
            var element = InnerList[index];
            if (element is Control control)
                Remove(control);
            else if (element is ArrangedElementWidget widget)
            {
                InnerList.RemoveAt(index);
                if (_ownerControl?.GetChildrenArray().Any(o => o.Equals(widget.GetWidget)) == true)
                    _ownerControl.RemoveChild(widget.GetWidget);
            }
        }

        /// <summary>
        /// Removes a child control identified by key (name).
        /// </summary>
        /// <param name="key">
        /// Key/name of the child to remove.
        /// </param>
        public virtual void RemoveByKey(string? key)
        {
            var index = IndexOfKey(key);
            if (IsValidIndex(index))
            {
                RemoveAt(index);
            }
        }

        /// <summary>
        /// Sets the child index (z-order) for the specified child control.
        /// </summary>
        /// <param name="child">
        /// Child control to reposition.
        /// </param>
        /// <param name="newIndex">
        /// New zero-based index.
        /// </param>
        public virtual void SetChildIndex(Control child, int newIndex) => SetChildIndexInternal(child, newIndex);

        internal virtual void SetChildIndexInternal(Control child, int newIndex)
        {
            // This was empty, so SetChildIndex never reordered anything.
            if (child is null)
            {
                return;
            }

            var current = InnerList.IndexOf(child);
            if (current < 0)
            {
                return;
            }

            var target = newIndex < 0 ? InnerList.Count - 1 : Math.Min(newIndex, InnerList.Count - 1);
            if (target == current)
            {
                return;
            }

            InnerList.RemoveAt(current);
            InnerList.Insert(target, child);
        }

        /// <summary>
        /// Returns the index of the given control in this collection.
        /// </summary>
        /// <param name="control">
        /// Control to locate.
        /// </param>
        /// <returns>
        /// Zero-based index or -1 if not found.
        /// </returns>
        public int IndexOf(Control? control) => ((IList)InnerList).IndexOf(control);

        /// <summary>
        /// Gets the child index for the specified control.
        /// </summary>
        /// <param name="child">
        /// Child control.
        /// </param>
        /// <returns>
        /// Index of the child.
        /// </returns>
        public int GetChildIndex(Control child) => GetChildIndex(child, true);

        /// <summary>
        /// Gets the child index for the specified control; optionally throw if not a child.
        /// </summary>
        /// <param name="child">
        /// Child control.
        /// </param>
        /// <param name="throwException">
        /// True to throw when control is not a child.
        /// </param>
        /// <returns>
        /// Index of the child.
        /// </returns>
        public virtual int GetChildIndex(Control child, bool throwException)
        {
            var index = IndexOf(child);
            if (index == -1 && throwException)
            {
                throw new ArgumentException("ControlNotChild");
            }

            return index;
        }

        int IList.Add(object? control)
        {
            if (control is Control c)
            {
                Add(c);
                var idx = ((IList)InnerList).IndexOf(c);
                return IsValidIndex(idx) ? idx : -1;
            }
            else
            {
                throw new ArgumentException("ControlBadControl", nameof(control));
            }
        }

        object ICloneable.Clone()
        {
            var ccOther = new ControlCollection(_owner, _ownerControl);
            ccOther.InnerList.AddRange(InnerList);

            return ccOther;
        }

        void IList.Remove(object? element)
        {
            if (element is Control control)
                Remove(control);
            else if (element is Gtk.Widget widget)
            {
                _ownerControl?.RemoveChild(widget);
                var index = this.IndexOfKey(widget.Name);
                if (index >= 0)
                    InnerList.RemoveAt(index);
            }
        }

        private bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < Count));
        }

        private void NativeAdd(object item)
        {
            try
            {
                if (item is Control icontrol)
                {
                    icontrol.Parent = _owner;
                }
                if (_ownerControl is Gtk.Overlay lay)
                {
                    if (item is StatusStrip statusbar)
                    {
                        if (_owner is Form form)
                        {
                            statusbar.self.Halign = Gtk.Align.Fill;
                            statusbar.self.Valign = Gtk.Align.Start;
                            statusbar.self.Hexpand = false;
                            statusbar.self.Vexpand = false;
                            statusbar.self.MarginStart = 0;
                            statusbar.self.MarginTop = 0;
                            statusbar.self.MarginEnd = 0;
                            statusbar.self.MarginBottom = 0;
                            if (lay.IsRealized)
                                statusbar.self.Show();
                            var overlay = new Gtk.Overlay
                            {
                                HeightRequest = statusbar.Height
                            };
                            overlay.AddOverlay(statusbar.self);
                            form.self.ContentArea.Append(overlay);
                        }
                    }
                    else if (item is Control control)
                    {
                        lay.AddOverlay(control.Widget);
                        if (control.Widget is Gtk.Label || control.Widget is Gtk.Button || control.Widget is Gtk.Entry || control.Widget is Gtk.TextView || control.Widget is Gtk.ScrolledWindow)
                            { } // Gtk4-removed: Overlay.SetOverlayPassThrough (pass-through is default)
                    }
                    else if (item is Gtk.Widget widget)
                    {
                        lay.AddOverlay(widget);
                    }
                }
                else if (_ownerControl is Gtk.Fixed lay2)
                {
                    if (item is Control con)
                    {
                        lay2.Put(con.Widget, 0, 0);
                    }
                    else if (item is Gtk.Widget widget)
                    {
                        lay2.Put(widget, 0, 0);
                    }
                }
                else if (_ownerControl is Gtk.Fixed lay3)
                {
                    if (item is Control con)
                    {
                        lay3.Put(con.Widget, 0, 0);
                    }
                    else if (item is Gtk.Widget widget)
                    {
                        lay3.Put(widget, 0, 0);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                if (_ownerControl?.IsRealized == true)
                {
                    if (item is Control con)
                        con.Widget?.Show();
                    else if (item is Gtk.Widget widget)
                        widget.Show();
                }
            }
        }

        private void NativeAdd()
        {
            try
            {
                foreach (var item in this)
                {
                    NativeAdd(item);
                }
            }
            catch
            {
                throw;
            }
        }

        private void OwnerContainer_Mapped(object? sender, EventArgs e)
        {
            if (sender is Gtk.Overlay lay)
            {
                if (_owner is ScrollableControl scrollableControl)
                {
                    scrollableControl.PerformLayout(scrollableControl, nameof(scrollableControl.AutoScroll));
                }

                var tabs = InnerList.OfType<Control>().ToList();
                tabs.Sort((a, b) => a.TabIndex.CompareTo(b.TabIndex));
                lay.ChildFocus(Gtk.DirectionType.TabForward);
                // Gtk4-removed: Container.FocusChain (tab order via widget order / focus API). TODO(gtk4).
            }
        }
    }
}
