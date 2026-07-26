// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a <see cref="ToolStrip" /> control that provides navigation and manipulation of the data on a
/// form that is bound to a <see cref="BindingSource" />. The standard items move through the underlying list
/// (first/previous/next/last), show and set the current position, add and delete rows, and stay enabled or
/// disabled according to the list's state.
/// </summary>
public class BindingNavigator : ToolStrip, ISupportInitialize
{
    private BindingSource? _bindingSource;
    private ToolStripItem? _moveFirstItem;
    private ToolStripItem? _movePreviousItem;
    private ToolStripItem? _moveNextItem;
    private ToolStripItem? _moveLastItem;
    private ToolStripItem? _addNewItem;
    private ToolStripItem? _deleteItem;
    private ToolStripTextBox? _positionItem;
    private ToolStripLabel? _countItem;
    private string _countItemFormat = "of {0}";
    private bool _initializing;

    private readonly EventHandler _onMoveFirst;
    private readonly EventHandler _onMovePrevious;
    private readonly EventHandler _onMoveNext;
    private readonly EventHandler _onMoveLast;
    private readonly EventHandler _onAddNew;
    private readonly EventHandler _onDelete;

    /// <summary>Initializes a new instance of the <see cref="BindingNavigator" /> class without standard items.</summary>
    public BindingNavigator() : this(false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingNavigator" /> class, optionally adding the
    /// standard set of navigation items.
    /// </summary>
    /// <param name="addStandardItems"><see langword="true" /> to add the standard items; otherwise, <see langword="false" />.</param>
    public BindingNavigator(bool addStandardItems) : base()
    {
        _onMoveFirst = OnMoveFirst;
        _onMovePrevious = OnMovePrevious;
        _onMoveNext = OnMoveNext;
        _onMoveLast = OnMoveLast;
        _onAddNew = OnAddNew;
        _onDelete = OnDelete;

        if (addStandardItems)
        {
            AddStandardItems();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingNavigator" /> class, bound to the specified
    /// <see cref="BindingSource" />, and adds the standard items.
    /// </summary>
    /// <param name="bindingSource">The <see cref="BindingSource" /> to bind to.</param>
    public BindingNavigator(BindingSource? bindingSource) : this(true)
    {
        BindingSource = bindingSource;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingNavigator" /> class and adds it to the specified
    /// container.
    /// </summary>
    /// <param name="container">The container that owns this navigator.</param>
    public BindingNavigator(IContainer container) : this(false)
    {
        container?.Add(this);
    }

    /// <summary>Gets or sets the <see cref="BindingSource" /> that is the source of data.</summary>
    public BindingSource? BindingSource
    {
        get => _bindingSource;
        set
        {
            WireUpBindingSource(value);
        }
    }

    /// <summary>Gets or sets the <see cref="ToolStripItem" /> that moves to the first item.</summary>
    public ToolStripItem? MoveFirstItem
    {
        get => _moveFirstItem;
        set => WireUpButton(ref _moveFirstItem, value, _onMoveFirst);
    }

    /// <summary>Gets or sets the <see cref="ToolStripItem" /> that moves to the previous item.</summary>
    public ToolStripItem? MovePreviousItem
    {
        get => _movePreviousItem;
        set => WireUpButton(ref _movePreviousItem, value, _onMovePrevious);
    }

    /// <summary>Gets or sets the <see cref="ToolStripItem" /> that moves to the next item.</summary>
    public ToolStripItem? MoveNextItem
    {
        get => _moveNextItem;
        set => WireUpButton(ref _moveNextItem, value, _onMoveNext);
    }

    /// <summary>Gets or sets the <see cref="ToolStripItem" /> that moves to the last item.</summary>
    public ToolStripItem? MoveLastItem
    {
        get => _moveLastItem;
        set => WireUpButton(ref _moveLastItem, value, _onMoveLast);
    }

    /// <summary>Gets or sets the <see cref="ToolStripItem" /> that adds a new item to the underlying list.</summary>
    public ToolStripItem? AddNewItem
    {
        get => _addNewItem;
        set => WireUpButton(ref _addNewItem, value, _onAddNew);
    }

    /// <summary>Gets or sets the <see cref="ToolStripItem" /> that deletes the current item.</summary>
    public ToolStripItem? DeleteItem
    {
        get => _deleteItem;
        set => WireUpButton(ref _deleteItem, value, _onDelete);
    }

    /// <summary>Gets or sets the <see cref="ToolStripTextBox" /> that displays and edits the current position.</summary>
    public ToolStripTextBox? PositionItem
    {
        get => _positionItem;
        set
        {
            if (_positionItem == value)
            {
                return;
            }

            if (_positionItem != null)
            {
                _positionItem.KeyUp -= OnPositionKeyUp;
                _positionItem.LostFocus -= OnPositionLostFocus;
            }

            _positionItem = value;

            if (_positionItem != null)
            {
                _positionItem.KeyUp += OnPositionKeyUp;
                _positionItem.LostFocus += OnPositionLostFocus;
            }

            RefreshItemsInternal();
        }
    }

    /// <summary>Gets or sets the <see cref="ToolStripLabel" /> that displays the total number of items.</summary>
    public ToolStripLabel? CountItem
    {
        get => _countItem;
        set
        {
            _countItem = value;
            RefreshItemsInternal();
        }
    }

    /// <summary>
    /// Gets or sets the format string applied to the number of items shown by the <see cref="CountItem" />.
    /// The default is <c>"of {0}"</c>.
    /// </summary>
    public string CountItemFormat
    {
        get => _countItemFormat;
        set
        {
            _countItemFormat = value;
            RefreshItemsInternal();
        }
    }

    /// <summary>Occurs when the state of the items is refreshed because the bound data changed.</summary>
    public event EventHandler? RefreshItems;

    /// <summary>Adds the standard set of navigation items to the <see cref="BindingNavigator" />.</summary>
    public virtual void AddStandardItems()
    {
        BeginInit();
        try
        {
            MoveFirstItem = new ToolStripButton("|<") { Name = "bindingNavigatorMoveFirstItem", ToolTipText = "Move first" };
            MovePreviousItem = new ToolStripButton("<") { Name = "bindingNavigatorMovePreviousItem", ToolTipText = "Move previous" };
            var separator1 = new ToolStripSeparator { Name = "bindingNavigatorSeparator" };
            PositionItem = new ToolStripTextBox { Name = "bindingNavigatorPositionItem", Width = 50, Text = "0" };
            CountItem = new ToolStripLabel { Name = "bindingNavigatorCountItem" };
            var separator2 = new ToolStripSeparator { Name = "bindingNavigatorSeparator1" };
            MoveNextItem = new ToolStripButton(">") { Name = "bindingNavigatorMoveNextItem", ToolTipText = "Move next" };
            MoveLastItem = new ToolStripButton(">|") { Name = "bindingNavigatorMoveLastItem", ToolTipText = "Move last" };
            var separator3 = new ToolStripSeparator { Name = "bindingNavigatorSeparator2" };
            AddNewItem = new ToolStripButton("+") { Name = "bindingNavigatorAddNewItem", ToolTipText = "Add new" };
            DeleteItem = new ToolStripButton("X") { Name = "bindingNavigatorDeleteItem", ToolTipText = "Delete" };

            Items.AddRange(new ToolStripItem[]
            {
                _moveFirstItem!, _movePreviousItem!, separator1,
                _positionItem!, _countItem!, separator2,
                _moveNextItem!, _moveLastItem!, separator3,
                _addNewItem!, _deleteItem!
            });
        }
        finally
        {
            EndInit();
        }
    }

    /// <summary>Raises the <see cref="RefreshItems" /> event.</summary>
    protected virtual void OnRefreshItems()
    {
        RefreshItems?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Updates the enabled state and text of the standard items from the current binding state.</summary>
    public virtual void RefreshItemsCore()
    {
        try
        {
            var count = _bindingSource?.Count ?? 0;
            var position = _bindingSource?.Position ?? -1;
            var allowNew = _bindingSource?.AllowNew ?? false;
            var allowRemove = _bindingSource?.AllowRemove ?? false;

            if (_moveFirstItem != null)
            {
                _moveFirstItem.Enabled = position > 0;
            }

            if (_movePreviousItem != null)
            {
                _movePreviousItem.Enabled = position > 0;
            }

            if (_moveNextItem != null)
            {
                _moveNextItem.Enabled = count > 0 && position < count - 1;
            }

            if (_moveLastItem != null)
            {
                _moveLastItem.Enabled = count > 0 && position < count - 1;
            }

            if (_addNewItem != null)
            {
                _addNewItem.Enabled = allowNew;
            }

            if (_deleteItem != null)
            {
                _deleteItem.Enabled = count > 0 && allowRemove;
            }

            if (_positionItem != null)
            {
                _positionItem.Text = position >= 0 ? (position + 1).ToString() : "0";
                _positionItem.Enabled = count > 0;
            }

            if (_countItem != null)
            {
                _countItem.Text = string.Format(_countItemFormat, count);
            }
        }
        catch
        {
            // Refreshing item state must never throw into the caller that mutated the list.
        }
    }

    /// <summary>Signals the object that initialization is starting.</summary>
    public void BeginInit()
    {
        _initializing = true;
    }

    /// <summary>Signals the object that initialization is complete.</summary>
    public void EndInit()
    {
        _initializing = false;
        RefreshItemsInternal();
    }

    /// <summary>Releases the resources used by the <see cref="BindingNavigator" />.</summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            WireUpBindingSource(null);
        }

        base.Dispose(disposing);
    }

    private void RefreshItemsInternal()
    {
        if (_initializing)
        {
            return;
        }

        OnRefreshItems();
        RefreshItemsCore();
    }

    private void WireUpButton(ref ToolStripItem? field, ToolStripItem? newItem, EventHandler clickHandler)
    {
        if (field == newItem)
        {
            return;
        }

        if (field != null)
        {
            field.Click -= clickHandler;
        }

        field = newItem;

        if (newItem != null)
        {
            newItem.Click += clickHandler;
        }

        RefreshItemsInternal();
    }

    private void WireUpBindingSource(BindingSource? newSource)
    {
        if (_bindingSource == newSource)
        {
            return;
        }

        if (_bindingSource != null)
        {
            _bindingSource.PositionChanged -= OnBindingSourceStateChanged;
            _bindingSource.CurrentChanged -= OnBindingSourceStateChanged;
            _bindingSource.ListChanged -= OnBindingSourceListChanged;
        }

        _bindingSource = newSource;

        if (_bindingSource != null)
        {
            _bindingSource.PositionChanged += OnBindingSourceStateChanged;
            _bindingSource.CurrentChanged += OnBindingSourceStateChanged;
            _bindingSource.ListChanged += OnBindingSourceListChanged;
        }

        RefreshItemsInternal();
    }

    private void OnBindingSourceStateChanged(object? sender, EventArgs e) => RefreshItemsInternal();

    private void OnBindingSourceListChanged(object? sender, ListChangedEventArgs e) => RefreshItemsInternal();

    private void OnMoveFirst(object? sender, EventArgs e)
    {
        _bindingSource?.MoveFirst();
        RefreshItemsInternal();
    }

    private void OnMovePrevious(object? sender, EventArgs e)
    {
        _bindingSource?.MovePrevious();
        RefreshItemsInternal();
    }

    private void OnMoveNext(object? sender, EventArgs e)
    {
        _bindingSource?.MoveNext();
        RefreshItemsInternal();
    }

    private void OnMoveLast(object? sender, EventArgs e)
    {
        _bindingSource?.MoveLast();
        RefreshItemsInternal();
    }

    private void OnAddNew(object? sender, EventArgs e)
    {
        _bindingSource?.AddNew();
        RefreshItemsInternal();
    }

    private void OnDelete(object? sender, EventArgs e)
    {
        _bindingSource?.RemoveCurrent();
        RefreshItemsInternal();
    }

    private void OnPositionKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            AcceptNewPosition();
        }
    }

    private void OnPositionLostFocus(object? sender, EventArgs e) => AcceptNewPosition();

    private void AcceptNewPosition()
    {
        if (_positionItem == null || _bindingSource == null)
        {
            return;
        }

        if (int.TryParse(_positionItem.Text, out var oneBased))
        {
            _bindingSource.Position = oneBased - 1;
        }

        RefreshItemsInternal();
    }
}
