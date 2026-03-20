/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 * date: 2024/1/3
 */
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
/// Represents a
/// <see cref="T:System.Windows.Forms.ToolStripComboBox" />
/// that is properly rendered in a
/// <see cref="T:System.Windows.Forms.ToolStrip" />
/// </summary>
public class ToolStripComboBox : WidgetToolStrip<Gtk.MenuItem>
{

    private readonly ObjectCollection __itemsData;

    /// <summary>
    /// Occurs when selected index changed.
    /// </summary>
    public virtual event EventHandler SelectedIndexChanged;

    /// <summary>
    /// Occurs when selected value changed.
    /// </summary>
    public virtual event EventHandler SelectedValueChanged;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ToolStripComboBox" /> class.
    /// </summary>
    public ToolStripComboBox() : base("ToolStripComboBox", null)
    {
        __itemsData = new ObjectCollection(this.comboBox);
        this.comboBox.Changed += ComboBox_Changed;
    }

    /// <summary>
    /// Gets or sets a value indicating whether formatting enabled.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if formatting enabled; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool FormattingEnabled
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>
    /// The items.
    /// </value>
    public new ObjectCollection Items { get { return __itemsData; } }

    /// <summary>
    /// Gets the index of the selected.
    /// </summary>
    /// <value>
    /// The index of the selected.
    /// </value>
    public int SelectedIndex { get { return base.comboBox.Active; } }

    /// <summary>
    /// Gets the selected item.
    /// </summary>
    /// <value>
    /// The selected item.
    /// </value>
    public object SelectedItem { get { return __itemsData[SelectedIndex]; } }

    /// <summary>
    /// Gets or sets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public override Size Size
    {
        get => base.Size;
        set { this.comboBox.WidthRequest = value.Width; this.comboBox.HeightRequest = value.Height; base.Size = value; }
    }

    /// <summary>
    /// Raises the SelectedIndexChanged event.
    /// </summary>
    protected virtual void OnSelectedIndexChanged(EventArgs e) { SelectedIndexChanged?.Invoke(this, e); }

    /// <summary>
    /// Raises the SelectedValueChanged event.
    /// </summary>
    protected virtual void OnSelectedValueChanged(EventArgs e) { SelectedValueChanged?.Invoke(this, e); }

    private void ComboBox_Changed(object sender, EventArgs e)
    {
        SelectedIndexChanged?.Invoke(this, e);
        SelectedValueChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Class ObjectCollection. Implements the
    /// <see cref="T:System.Collections.ArrayList" />
    /// </summary>
    /// <seealso cref="T:System.Collections.ArrayList" />
    [ListBindable(false)]
    public class ObjectCollection : ArrayList
    {
        readonly Gtk.ComboBoxText __owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.ToolStripComboBox.ObjectCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public ObjectCollection(Gtk.ComboBoxText owner)
        {
            __owner = owner;
        }

        /// <summary>
        /// Adds an object to the end of the
        /// <see cref="T:System.Collections.ArrayList">
        /// </see>.
        /// </summary>
        /// <param name="value">
        /// The
        /// <see cref="T:System.Object">
        /// </see> to be added to the end of the
        /// <see cref="T:System.Collections.ArrayList">
        /// </see>. The value can be null.
        /// </param>
        /// <returns>
        /// The
        /// <see cref="T:System.Collections.ArrayList">
        /// </see> index at which the
        /// <paramref name="value">
        /// value
        /// </paramref> has been added.
        /// </returns>
        public override int Add(object value)
        {
            __owner.AppendText(value?.ToString());

            return base.Add(value);
        }

        /// <summary>
        /// Adds the elements of an
        /// <see cref="T:System.Collections.ICollection">
        /// </see> to the end of the
        /// <see cref="T:System.Collections.ArrayList">
        /// </see>.
        /// </summary>
        /// <param name="c">
        /// The
        /// <see cref="T:System.Collections.ICollection">
        /// </see> whose elements should be added to the end of the
        /// <see cref="T:System.Collections.ArrayList">
        /// </see>. The collection itself cannot be null, but it can contain elements that are null.
        /// </param>
        public override void AddRange(ICollection c)
        {
            foreach (var item in c)
                __owner.AppendText(item.ToString());
            base.AddRange(c);
        }

        /// <summary>
        /// Removes all elements from the
        /// <see cref="T:System.Collections.ArrayList">
        /// </see>.
        /// </summary>
        public override void Clear()
        {
            __owner.Clear();
            base.Clear();
        }

    }
}