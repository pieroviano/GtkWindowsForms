using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
/// Represents a status bar control.
/// </summary>
/// <remarks>
/// The legacy status API was missing entirely. <see cref="T:System.Windows.Forms.StatusBar" /> derives from
/// <see cref="T:System.Windows.Forms.StatusStrip" /> so that it is a real control which docks and renders,
/// while exposing the <c>Panels</c> collection the old API uses.
/// </remarks>
public class StatusBar : StatusStrip
{
    private StatusBarPanelCollection? _panels;

    /// <summary>
    /// Occurs when a panel is clicked.
    /// </summary>
    public event StatusBarPanelClickEventHandler? PanelClick;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:System.Windows.Forms.StatusBar" /> class.
    /// </summary>
    public StatusBar()
    {
    }

    /// <summary>
    /// Gets the panels displayed on the status bar.
    /// </summary>
    public StatusBarPanelCollection Panels => _panels ??= new StatusBarPanelCollection(this);

    /// <summary>
    /// Gets or sets a value indicating whether panels are shown instead of a single caption.
    /// </summary>
    [DefaultValue(false)]
    public bool ShowPanels
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether a sizing grip is shown.
    /// </summary>
    [DefaultValue(true)]
    public new bool SizingGrip
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.StatusBar.PanelClick" /> event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e) => PanelClick?.Invoke(this, e);

    /// <summary>
    /// Represents the collection of panels on a <see cref="T:System.Windows.Forms.StatusBar" />.
    /// </summary>
    public class StatusBarPanelCollection : IList
    {
        private readonly List<StatusBarPanel> items = new();

        private readonly StatusBar owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.StatusBar.StatusBarPanelCollection" /> class.
        /// </summary>
        /// <param name="owner">The status bar that owns the collection.</param>
        public StatusBarPanelCollection(StatusBar owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        /// <summary>Gets the number of panels.</summary>
        public int Count => items.Count;

        /// <summary>Gets a value indicating whether the collection is read-only.</summary>
        public bool IsReadOnly => false;

        /// <summary>Gets a value indicating whether the collection has a fixed size.</summary>
        public bool IsFixedSize => false;

        /// <summary>Gets a value indicating whether access to the collection is synchronised.</summary>
        public bool IsSynchronized => false;

        /// <summary>Gets an object that can be used to synchronise access to the collection.</summary>
        public object SyncRoot => ((ICollection)items).SyncRoot;

        /// <summary>Gets or sets the panel at the specified position.</summary>
        /// <param name="index">The zero-based position of the panel.</param>
        public virtual StatusBarPanel this[int index]
        {
            get => items[index];
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        /// <summary>Gets the panel with the specified name, or <see langword="null" />.</summary>
        /// <param name="key">The name to look for.</param>
        public virtual StatusBarPanel? this[string key]
        {
            get
            {
                var index = IndexOfKey(key);

                return index == -1 ? null : items[index];
            }
        }

        object? IList.this[int index]
        {
            get => items[index];
            set => this[index] = value as StatusBarPanel
                                 ?? throw new ArgumentException("Value must be a StatusBarPanel.", nameof(value));
        }

        /// <summary>Adds a panel displaying the given text.</summary>
        /// <param name="text">The text of the new panel.</param>
        /// <returns>The panel that was added.</returns>
        public virtual StatusBarPanel Add(string text) => Add(new StatusBarPanel { Text = text });

        /// <summary>Adds the specified panel.</summary>
        /// <param name="value">The panel to add.</param>
        /// <returns>The panel that was added.</returns>
        public virtual StatusBarPanel Add(StatusBarPanel value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            items.Add(value);
            value.Parent = owner;
            owner.Items.Add(value.Label);

            return value;
        }

        int IList.Add(object? value)
        {
            Add(value as StatusBarPanel ?? throw new ArgumentException("Value must be a StatusBarPanel.", nameof(value)));

            return items.Count - 1;
        }

        /// <summary>Adds every one of the given panels.</summary>
        /// <param name="panels">The panels to add.</param>
        public virtual void AddRange(StatusBarPanel[] panels)
        {
            if (panels == null)
            {
                throw new ArgumentNullException(nameof(panels));
            }

            foreach (var panel in panels)
            {
                Add(panel);
            }
        }

        /// <summary>Removes every panel.</summary>
        public virtual void Clear()
        {
            foreach (var panel in items)
            {
                panel.Parent = null;
                owner.Items.Remove(panel.Label);
            }

            items.Clear();
        }

        /// <summary>Indicates whether the specified panel is in the collection.</summary>
        /// <param name="panel">The panel to look for.</param>
        public bool Contains(StatusBarPanel panel) => items.Contains(panel);

        bool IList.Contains(object? value) => value is StatusBarPanel panel && items.Contains(panel);

        /// <summary>Indicates whether a panel with the specified name is present.</summary>
        /// <param name="key">The name to look for.</param>
        public virtual bool ContainsKey(string? key) => IndexOfKey(key) != -1;

        /// <summary>Copies the panels into the given array.</summary>
        /// <param name="dest">The array to copy into.</param>
        /// <param name="index">The position in the array to start at.</param>
        public void CopyTo(Array dest, int index) => ((ICollection)items).CopyTo(dest, index);

        /// <summary>Returns an enumerator over the panels.</summary>
        public IEnumerator GetEnumerator() => items.GetEnumerator();

        /// <summary>Returns the position of the specified panel, or -1.</summary>
        /// <param name="panel">The panel to locate.</param>
        public int IndexOf(StatusBarPanel panel) => items.IndexOf(panel);

        int IList.IndexOf(object? value) => value is StatusBarPanel panel ? items.IndexOf(panel) : -1;

        /// <summary>Returns the position of the panel with the specified name, or -1.</summary>
        /// <param name="key">The name to look for.</param>
        public virtual int IndexOfKey(string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }

            return items.FindIndex(panel => string.Equals(panel.Name, key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Inserts a panel at the given position.</summary>
        /// <param name="index">The position to insert at.</param>
        /// <param name="value">The panel to insert.</param>
        public virtual void Insert(int index, StatusBarPanel value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            items.Insert(index, value);
            value.Parent = owner;
            owner.Items.Insert(index, value.Label);
        }

        void IList.Insert(int index, object? value) =>
            Insert(index, value as StatusBarPanel
                          ?? throw new ArgumentException("Value must be a StatusBarPanel.", nameof(value)));

        /// <summary>Removes the specified panel.</summary>
        /// <param name="value">The panel to remove.</param>
        public virtual void Remove(StatusBarPanel value)
        {
            if (value == null || !items.Remove(value))
            {
                return;
            }

            value.Parent = null;
            owner.Items.Remove(value.Label);
        }

        void IList.Remove(object? value)
        {
            if (value is StatusBarPanel panel)
            {
                Remove(panel);
            }
        }

        /// <summary>Removes the panel at the given position.</summary>
        /// <param name="index">The position of the panel to remove.</param>
        public virtual void RemoveAt(int index) => Remove(items[index]);

        /// <summary>Removes the panel with the specified name, if there is one.</summary>
        /// <param name="key">The name of the panel to remove.</param>
        public virtual void RemoveByKey(string? key)
        {
            var index = IndexOfKey(key);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }
    }
}
