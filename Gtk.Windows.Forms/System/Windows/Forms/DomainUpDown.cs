// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using GtkDomainEntry = GtkSystem.Windows.Forms.GtkControls.ControlBase.TextBoxBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows spin box (also known as an up-down control) that displays string values from a list.
/// </summary>
/// <remarks>
/// The item model, selection and wrap/sort behaviour follow WinForms exactly. The control is backed by a
/// read-only GTK entry that shows the selected item; the up and down buttons are driven programmatically
/// through <see cref="M:System.Windows.Forms.DomainUpDown.UpButton" /> and
/// <see cref="M:System.Windows.Forms.DomainUpDown.DownButton" /> (GTK does not provide a composite text +
/// spin-button widget, so the arrows are not rendered as separate glyphs — a documented visual deviation).
/// </remarks>
[DesignerCategory("Component")]
public partial class DomainUpDown : UpDownBase
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly GtkDomainEntry self = new();

    private readonly DomainUpDownItemCollection _items;
    private int _selectedIndex = -1;
    private bool _sorted;
    private bool _wrap;

    /// <summary>
    /// Occurs when the <see cref="P:System.Windows.Forms.DomainUpDown.SelectedItem" /> property has been
    /// changed.
    /// </summary>
    public event EventHandler? SelectedItemChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.DomainUpDown" /> class.
    /// </summary>
    public DomainUpDown() : base()
    {
        self.Override.sender = this;
        self.IsEditable = false;
        _items = new DomainUpDownItemCollection(this);
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    public override object GtkControl => self;

    /// <summary>
    /// Gets the object collection assigned to this up-down control.
    /// </summary>
    public DomainUpDownItemCollection Items => _items;

    /// <summary>
    /// Gets or sets the text displayed in the up-down control.
    /// </summary>
    public override string Text
    {
        get => self.Text;
        set => self.Text = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the item collection is sorted.
    /// </summary>
    public bool Sorted
    {
        get => _sorted;
        set
        {
            _sorted = value;
            if (_sorted)
            {
                _items.SortInternal();
                UpdateDisplay();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the collection of items continues to the first or last item
    /// if the user continues past the end of the list.
    /// </summary>
    public bool Wrap
    {
        get => _wrap;
        set => _wrap = value;
    }

    /// <summary>
    /// Gets or sets the index value of the selected item.
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value < -1 || value >= _items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            if (_selectedIndex != value)
            {
                _selectedIndex = value;
                UpdateDisplay();
                OnSelectedItemChanged(EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected item based on the index value of the selected item in the collection.
    /// </summary>
    public object? SelectedItem
    {
        get => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;
        set
        {
            var index = value == null ? -1 : _items.IndexOf(value);
            SelectedIndex = index;
        }
    }

    /// <summary>
    /// Displays the previous item in the collection.
    /// </summary>
    public override void UpButton()
    {
        MoveSelection(-1);
    }

    /// <summary>
    /// Displays the next item in the collection.
    /// </summary>
    public override void DownButton()
    {
        MoveSelection(1);
    }

    /// <summary>
    /// Returns a string that represents the <see cref="T:System.Windows.Forms.DomainUpDown" /> control.
    /// </summary>
    /// <returns>A string that includes the item count and selected index.</returns>
    public override string ToString()
    {
        return $"{base.ToString()}, Items.Count: {_items.Count}, SelectedIndex: {_selectedIndex}";
    }

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.DomainUpDown.SelectedItemChanged" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> that contains the event data.</param>
    protected virtual void OnSelectedItemChanged(EventArgs e) => SelectedItemChanged?.Invoke(this, e);

    internal void OnItemsChanged()
    {
        if (_sorted)
        {
            _items.SortInternal();
        }

        if (_selectedIndex >= _items.Count)
        {
            _selectedIndex = _items.Count - 1;
        }

        UpdateDisplay();
    }

    private void MoveSelection(int delta)
    {
        if (_items.Count == 0)
        {
            return;
        }

        var next = _selectedIndex + delta;
        if (next < 0)
        {
            next = _wrap ? _items.Count - 1 : 0;
        }
        else if (next >= _items.Count)
        {
            next = _wrap ? 0 : _items.Count - 1;
        }

        SelectedIndex = next;
    }

    private void UpdateDisplay()
    {
        self.Text = SelectedItem?.ToString() ?? string.Empty;
    }
}

/// <summary>
/// Encapsulates a collection of objects for use by the <see cref="T:System.Windows.Forms.DomainUpDown" />
/// class.
/// </summary>
public class DomainUpDownItemCollection : ArrayList
{
    private readonly DomainUpDown _owner;

    internal DomainUpDownItemCollection(DomainUpDown owner)
    {
        _owner = owner;
    }

    /// <summary>Gets or sets an item at the specified index in the collection.</summary>
    /// <param name="index">The zero-based index of the item.</param>
    /// <returns>The item at <paramref name="index" />.</returns>
    public override object? this[int index]
    {
        get => base[index];
        set
        {
            base[index] = value;
            _owner.OnItemsChanged();
        }
    }

    /// <summary>Adds an item to the end of the collection.</summary>
    /// <param name="value">The item to add.</param>
    /// <returns>The index at which the item was added.</returns>
    public override int Add(object? value)
    {
        var result = base.Add(value);
        _owner.OnItemsChanged();
        return result;
    }

    /// <summary>Inserts an item into the collection at the specified index.</summary>
    /// <param name="index">The zero-based index at which to insert.</param>
    /// <param name="value">The item to insert.</param>
    public override void Insert(int index, object? value)
    {
        base.Insert(index, value);
        _owner.OnItemsChanged();
    }

    /// <summary>Removes the specified item from the collection.</summary>
    /// <param name="obj">The item to remove.</param>
    public override void Remove(object? obj)
    {
        base.Remove(obj);
        _owner.OnItemsChanged();
    }

    /// <summary>Removes the item at the specified index.</summary>
    /// <param name="index">The zero-based index of the item to remove.</param>
    public override void RemoveAt(int index)
    {
        base.RemoveAt(index);
        _owner.OnItemsChanged();
    }

    /// <summary>Removes all items from the collection.</summary>
    public override void Clear()
    {
        base.Clear();
        _owner.OnItemsChanged();
    }

    internal void SortInternal()
    {
        base.Sort(Comparer<object?>.Create((a, b) =>
            string.Compare(a?.ToString(), b?.ToString(), StringComparison.CurrentCulture)));
    }
}
