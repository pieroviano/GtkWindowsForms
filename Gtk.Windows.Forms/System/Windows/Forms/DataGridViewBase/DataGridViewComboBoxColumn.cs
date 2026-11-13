/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Collections;
using System.Windows.Forms.GtkRender;
using Gtk;

namespace System.Windows.Forms;

/// <summary>
/// Represents a column of
/// <see cref="T:System.Windows.Forms.DataGridViewComboBoxCell" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class DataGridViewComboBoxColumn : DataGridViewColumn
{

    internal Gtk.ListStore model = new(typeof(string));

    readonly ObjectCollection _items;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewComboBoxColumn" /> class.
    /// </summary>
    public DataGridViewComboBoxColumn() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewComboBoxColumn" /> class attached to the specified grid.
    /// </summary>
    /// <param name="ownerGridView">
    /// The owning
    /// <see cref="T:System.Windows.Forms.DataGridView" />.
    /// </param>
    public DataGridViewComboBoxColumn(DataGridView? ownerGridView) : base(0, ownerGridView, new DataGridViewComboBoxCell())
    {
        _items = new ObjectCollection(this);
        var renderer = new CellRendererComboValue(this)
        {
            Editable = true,
            Mode = CellRendererMode.Editable
        };
        renderer.Edited += Renderer_Edited;
        renderer.EditingStarted += Renderer_EditingStarted;
        renderer.TextColumn = 0;
        renderer.Height = RowHeight;
        renderer.Model = model;
        base.PackStart(renderer, true);
        _cellRenderer = renderer;
    }

    /// <summary>
    /// Gets the collection of items presented by the combo box cells.
    /// </summary>
    public ObjectCollection Items => _items;

    internal override DataGridViewCell? NewCell(object? value = null, Type? valueType = null)
    {
        var newcell = new DataGridViewComboBoxCell();
        AtrributesClone(newcell);
        newcell.Value = value;
        newcell.ValueType = valueType;

        return newcell;
    }

    private void Renderer_Edited(object o, EditedArgs args)
    {
        RendererEdited(args.Path, args.NewText);
    }

    private void Renderer_EditingStarted(object o, EditingStartedArgs args)
    {
        _cellEditing = true;
        _cellEditablePath = args.Path;
        CellEditable = args.Editable;
    }

    /// <summary>
    /// Simple collection class for combo box items used by the column.
    /// </summary>
    public class ObjectCollection : ArrayList
    {

        private readonly DataGridViewComboBoxColumn _owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.DataGridViewComboBoxColumn.ObjectCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// Owning combo box column.
        /// </param>
        public ObjectCollection(DataGridViewComboBoxColumn owner)
        {
            _owner = owner;
        }

        /// <summary>
        /// Adds an item to the collection and the underlying GTK model.
        /// </summary>
        /// <param name="value">
        /// Item to add.
        /// </param>
        /// <returns>
        /// The index at which the item was added.
        /// </returns>
        public override int Add(object value)
        {
            _owner.model.AppendValues(value);

            return base.Add(value);
        }

        /// <summary>
        /// Adds a range of items to the collection.
        /// </summary>
        /// <param name="items">
        /// Array of items to add.
        /// </param>
        public void AddRange(object[] items)
        {
            foreach (var item in items)
                Add(item);
        }
    }
}