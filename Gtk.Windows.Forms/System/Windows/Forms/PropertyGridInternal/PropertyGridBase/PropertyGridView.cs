/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component Gtk.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using Cairo;
using Gdk;
using GLib;
using Gtk;

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed partial class PropertyGridView
{

    public Gtk.TreeStore store = new(typeof(GridEntry));

    public Gtk.TreeView tree = new();

    private object? _selectedObject;

    private GridEntry? oldSelectedPropertyItem;

    /// <summary>
    /// The cell editor currently open on the value column, if any. GTK hands it to us on
    /// <c>editing-started</c> and takes it away again on <c>edited</c> / <c>editing-canceled</c>; it is what
    /// <see cref="EnsurePendingChangesCommitted" /> and <see cref="DropDownDone" /> act on.
    /// </summary>
    private Gtk.ICellEditable? _activeEditable;

    public event PropertyValueChangedEventHandler? PropertyValueChanged;

    public event SelectedGridItemChangedEventHandler? SelectedGridItemChanged;

    public PropertyGridView(PropertyGrid propertyGrid)
    {
        OwnerGrid = propertyGrid;
        tree.ShowExpanders = false;
        tree.EnableTreeLines = false;
        tree.EnableGridLines = TreeViewGridLines.Horizontal;
        var iconTheme = new IconTheme();
        var column1 = new Gtk.TreeViewColumn
        {
            FixedWidth = 30,
            Resizable = false,
            Clickable = false,
            SortIndicator = false,
            Reorderable = false
        };
        var button1 = column1.Button as Gtk.Button;
        if (button1?.Child is Gtk.Box otherBox)
        {
            var img = new Gtk.Image(this.GetType().Assembly, "Resources.System.PBCategory.ico")
            {
                Visible = true
            };
            otherBox.Append(img);
            otherBox.ReorderChildAfter(img, null);
        }

        var expander = new CellRendererExpander(this)
        {
            CellBackgroundRgba = new RGBA() { Alpha = 0.6f, Red = 0.9f, Green = 0.9f, Blue = 0.9f }
        };
        column1.PackStart(expander, false);
        column1.AddAttribute(expander, "icon", 0);

        var expanderColumn = new Gtk.TreeViewColumn
        {
            MinWidth = 20,
            FixedWidth = 150,
            Resizable = true,
            Clickable = false,
            Title = " ",
            SortIndicator = true,
            SortOrder = SortType.Ascending,
            Reorderable = false
        };
        expanderColumn.Button.Name = "key";
        var expanderColumnButton = expanderColumn.Button as Gtk.Button;
        if (expanderColumnButton?.Child is Gtk.Box box)
        {
            var img = new Gtk.Image(this.GetType().Assembly, "Resources.System.PBAlpha.ico")
            {
                Visible = true
            };
            box.Append(img);
            box.ReorderChildAfter(img, null);
            // Gtk4 removed Box.SetChildPacking; the per-child expand/fill it set is expressed directly on the
            // child in Gtk4. The icon should sit at the start and not grow, so the sibling label takes the
            // slack.
            img.Halign = Gtk.Align.Start;
            img.Hexpand = false;
        }
        CellRendererText names = new CellRendererProperty(this)
        {
            Editable = false
        };
        expanderColumn.SortColumnId = 0;
        expanderColumn.PackStart(names, false);
        expanderColumn.AddAttribute(names, "otext", 0);

        var column3 = new Gtk.TreeViewColumn
        {
            MinWidth = 10,
            Resizable = false,
            Clickable = false,
            Title = " ",
            SortIndicator = false,
            Reorderable = false
        };
        column3.Button.Name = "value";
        Gtk.CellRendererText values = new CellRendererProperty(this)
        {
            Editable = true
        };
        column3.PackStart(values, false);
        column3.AddAttribute(values, "ovalue", 0);

        tree.AppendColumn(column1);
        tree.AppendColumn(expanderColumn);
        tree.AppendColumn(column3);
        tree.ExpanderColumn = expanderColumn;

        tree.LevelIndentation = 15;
        tree.Selection.Mode = Gtk.SelectionMode.Single;
        tree.ActivateOnSingleClick = true;
        tree.Selection.Changed += Selection_Changed;

        store.SetSortFunc(0, new TreeIterCompareFunc((model, iter1, iter2) =>
        {
            var val1 = model.GetValue(iter1, 0) as GridEntry;
            var val2 = model.GetValue(iter2, 0) as GridEntry;
            if (val1?.Label == null || val2?.Label == null)
                return 0;
            else
                return string.Compare(val1.Label, val2.Label, StringComparison.Ordinal);
        }));

        tree.Model = store;
    }

    public PropertyGrid OwnerGrid
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether the view is currently writing a value back to the selected object. Backs
    /// <see cref="P:System.Windows.Forms.PropertyGrid.InPropertySet" />.
    /// </summary>
    internal bool InPropertySet
    {
        get;
        private set;
    }

    /// <summary>
    /// Whether a cell editor is open on the value column.
    /// </summary>
    internal bool IsEditing => _activeEditable != null;

    /// <summary>
    /// Records the editor GTK has just opened on the value column.
    /// </summary>
    private void OnEditingStarted(Gtk.ICellEditable? editable) => _activeEditable = editable;

    /// <summary>
    /// Forgets the editor GTK has just closed.
    /// </summary>
    private void OnEditingFinished() => _activeEditable = null;

    /// <summary>
    /// Commits the edit in progress, if there is one. Mirrors
    /// <c>IComPropertyBrowser.EnsurePendingChangesCommitted</c>: with nothing being edited there is nothing
    /// to commit and the call succeeds.
    /// </summary>
    /// <returns>
    /// <see langword="false" /> only when an edit was open and committing it failed.
    /// </returns>
    internal bool EnsurePendingChangesCommitted()
    {
        var editable = _activeEditable;
        if (editable == null)
        {
            return true;
        }

        try
        {
            // Ends the edit the way GTK itself does when the entry loses focus: the editor raises
            // "editing-done", which drives the renderer's "edited" signal and so the value write.
            editable.FinishEditing();

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.GtkLogger.Debug("Unexpected exception committing a pending property edit", ex);

            return false;
        }
        finally
        {
            _activeEditable = null;
        }
    }

    /// <summary>
    /// Closes the open cell editor - GTK's equivalent of the drop-down the WinForms property browser hosts -
    /// committing whatever it holds, and returns focus to the grid.
    /// </summary>
    internal void DropDownDone()
    {
        var editable = _activeEditable;
        if (editable != null)
        {
            try
            {
                editable.FinishEditing();
                editable.RemoveWidget();
            }
            catch (Exception ex)
            {
                System.Diagnostics.GtkLogger.Debug("Unexpected exception closing the property editor", ex);
            }
            finally
            {
                _activeEditable = null;
            }
        }

        FocusGrid();
    }

    /// <summary>
    /// Moves keyboard focus to the property list, which is what the property browser does when the shell
    /// asks it to activate (F4).
    /// </summary>
    internal void FocusGrid()
    {
        if (tree.IsRealized)
        {
            tree.GrabFocus();
        }
    }

    public void LoadPropertyInfo(object? entryobj)
    {
        tree.Selection.UnselectAll();
        store.Clear();
        if (entryobj == null)
        {
            return;
        }
        _selectedObject = entryobj;
        var categorys = new List<GridEntry>();
        var propertyInfos = entryobj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var propertyInfo in propertyInfos)
        {
            // A property is browsable unless it says otherwise: BrowsableAttribute defaults to true, and an
            // undecorated property is the common case. Requiring the attribute meant an ordinary object -
            // one that carries no design-time attributes at all - showed no properties whatsoever.
            var attri = propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute));
            if (((BrowsableAttribute?)attri?.FirstOrDefault())?.Browsable == false)
            {
                continue;
            }

            var category = "【 uncategorized group】";
            var description = "";

            var attri2 = propertyInfo.GetCustomAttributes(typeof(CategoryAttribute));
            if (attri2 != null && attri2.Count() > 0)
            {
                var attribute = (CategoryAttribute)attri2.FirstOrDefault();
                category = attribute.Category;
            }
            var attri3 = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute));
            if (attri3 != null && attri3.Count() > 0)
            {
                var attribute = (DescriptionAttribute)attri3.FirstOrDefault();
                description = attribute.Description;
            }

            var propertyItem = new GridEntry(null, GridItemType.Category, 0, category, category, description)
            {
                Level = 0,
                Editable = false,
                PropertyInfo = propertyInfo,
                OwnerGrid = OwnerGrid
            };
            categorys.Add(propertyItem);
        }
        var gs = categorys.GroupBy(o => o.Label).OrderBy(g => g.Key);
        if (gs.Count() == 1 && gs.First().Key == "【 uncategorized group】")
        {
            GetPropertyInfos(null, null, entryobj);
        }
        else
        {
            foreach (var g in gs)
            {
                var value0 = new GridEntry(null, GridItemType.Category, 0, g.Key, g.Key, g.Key) { Level = 0, Editable = false, OwnerGrid = OwnerGrid };
                var category1 = store.AppendValues(value0);
                foreach (var m in g)
                {
                    GetPropertyInfo(category1, value0, m.PropertyInfo, entryobj);
                }
                tree.ExpandToPath(store.GetPath(category1));
            }
        }
    }

    private GridEntry? GetPropertyInfo(Gtk.TreeIter? parent, GridEntry? parentitem, PropertyInfo? property, object obj)
    {
        if (property?.CanRead == true)
        {
            // As in LoadPropertyInfo: no BrowsableAttribute means browsable, not hidden.
            var attri = property.GetCustomAttributes(typeof(BrowsableAttribute));
            if (((BrowsableAttribute?)attri?.FirstOrDefault())?.Browsable == false)
            {
                return null;
            }

            if (parent != null && property.CanWrite == false)
            {
                return null;
            }
            var description = "";
            var attri3 = property.GetCustomAttributes(typeof(DescriptionAttribute));
            if (attri3 != null && attri3.Count() > 0)
            {
                var attribute = (DescriptionAttribute)attri3.FirstOrDefault();
                description = attribute.Description;
            }
            if (property.PropertyType.IsEnum || property.PropertyType.Name == "String")
            {
                var value1 = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, obj, description) { Editable = property.CanWrite, ValueType = property.PropertyType, PropertyInfo = property, OwnerGrid = OwnerGrid };
                StoreValue(parent, value1);
                return value1;
            }
            else if (property.PropertyType.IsValueType)
            {
                if (property.PropertyType.IsPrimitive == false)
                { // only primitive types can be edited
                    var value1 = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, obj, description) { ValueType = property.PropertyType, PropertyInfo = property, OwnerGrid = OwnerGrid };
                    var node1 = StoreValue(parent, value1);
                    var val = property.GetValue(obj);
                    if (property.CanWrite)
                        GetPropertyInfos(node1, value1, val);
                    return value1;
                }
                else
                {
                    var value1 = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, obj, description) { Editable = property.CanWrite, ValueType = property.PropertyType, PropertyInfo = property, OwnerGrid = OwnerGrid };
                    var node1 = StoreValue(parent, value1);
                    return value1;
                }
            }
            else
            {
                var value1 = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, obj, description) { Editable = false, ValueType = property.PropertyType, PropertyInfo = property, OwnerGrid = OwnerGrid };
                var node1 = StoreValue(parent, value1);
                return value1;
            }
        }

        return null;
    }

    private void GetPropertyInfos(Gtk.TreeIter? parent, GridEntry? parentitem, object obj)
    {
        var type = obj.GetType();
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
        foreach (var property in properties)
        {
            GetPropertyInfo(parent, parentitem, property, obj);
        }
    }

    /// <summary>
    /// Raises the PropertyValueChanged event.
    /// </summary>
    private void OnPropertyValueChanged(System.Windows.Forms.PropertyValueChangedEventArgs e)
    {
        PropertyValueChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the SelectedGridItemChanged event.
    /// </summary>
    private void OnSelectedGridItemChanged(System.Windows.Forms.SelectedGridItemChangedEventArgs e)
    {
        SelectedGridItemChanged?.Invoke(this, e);
    }

    private void Selection_Changed(object sender, EventArgs e)
    {
        if (tree.IsSensitive && tree.Selection.GetSelected(out var model, out var oiter))
        {
            var propertyItem = store.GetValue(oiter, 0) as GridEntry;
            // Funnel through OnSelectedGridItemChanged instead of raising the event directly, so the
            // single raise-point is honoured and the event sender is this grid view.
            OnSelectedGridItemChanged(new SelectedGridItemChangedEventArgs(oldSelectedPropertyItem, propertyItem));
            oldSelectedPropertyItem = propertyItem;
        }
    }

    private Gtk.TreeIter StoreValue(Gtk.TreeIter? parent, object value)
    {
        if (parent == null)
            return store.AppendValues(value);
        else
            return store.AppendValues(parent.Value, value);
    }

    public class CellRendererExpander : Gtk.CellRendererToggle
    {
        readonly Pixbuf godownixbuf = new(typeof(CellRendererExpander).Assembly, "Resources.System.go-down-symbolic.png");

        readonly Pixbuf gonextpixbuf = new(typeof(CellRendererExpander).Assembly, "Resources.System.go-next-symbolic.png");

        public CellRendererExpander(PropertyGridView owner)
        {

        }

        [Property("icon")]
        public GridEntry Icon
        {
            set
            {

            }
        }

        protected override bool OnActivate(Event evnt, Widget widget, string path, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
        {
            var re = base.OnActivate(evnt, widget, path, background_area, cell_area, flags);
            var tree = widget as Gtk.TreeView;
            if (this.IsExpanded)
                tree?.CollapseRow(new TreePath(path));
            else
                tree?.ExpandToPath(new TreePath(path));

            return re;
        }

        protected override void OnSnapshot(Gtk.Snapshot snapshot, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
        {
            // Gtk4 replaced CellRenderer.OnRender(Cairo) with the snapshot vfunc. Append a Cairo context over
            // the cell and paint the expand/collapse arrow (the removed Gtk3 body drew only the icon, not the
            // toggle's checkbox, so base is deliberately not chained).
            var rect = Graphene.Rect.Alloc();
            rect.Init(background_area.X, background_area.Y, background_area.Width, background_area.Height);
            using var cr = snapshot.AppendCairo(rect);
            Gdk.CairoHelper.SetSourcePixbuf(cr, IsExpanded ? godownixbuf : gonextpixbuf, background_area.X + 10, background_area.Y + 5);
            cr.Paint();
        }
    }

    public class CellRendererProperty : Gtk.CellRendererCombo
    {
        readonly Gtk.ListStore model = new(typeof(string));

        readonly PropertyGridView owner;

        private GridEntry? _value;

        public CellRendererProperty(PropertyGridView owner)
        {
            this.owner = owner;
            this.TextColumn = 0;
            this.Height = 20;
            this.SetFixedSize(50, 20);
            this.Model = model;
            this.EditingStarted += CellRendererText_EditingStarted;
            this.Edited += CellRendererText_Edited;
            this.EditingCanceled += CellRendererText_EditingCanceled;
        }

        [Property("otext")]
        public GridEntry? OText
        {
            set
            {
                _value = value;
                if (_value != null)
                {
                    base.Text = value?.Label;
                    base.Editable = false;
                    base.Weight = value?.Level == 0 ? 900 : 200;
                }
            }

            get { return _value; }
        }

        [Property("ovalue")]
        public GridEntry? OValue
        {
            set
            {
                _value = value;
                if (_value != null)
                {
                    base.Text = value?.Value?.ToString();
                    base.Editable = value?.Editable ?? false;
                    base.Weight = value?.Level == 0 ? 900 : 200;
                }
            }

            get { return _value; }
        }

        protected override void OnSnapshot(Gtk.Snapshot snapshot, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
        {
            // Gtk4 snapshot vfunc replacing the removed OnRender(Cairo). Top-level (category) rows get a
            // translucent grey wash behind their text; the base renderer then draws the text itself.
            if (_value?.Level == 0)
            {
                var rect = Graphene.Rect.Alloc();
                rect.Init(background_area.X, background_area.Y, background_area.Width, background_area.Height);
                using var cr = snapshot.AppendCairo(rect);
                cr.SetSourceRGBA(0.9, 0.9, 0.9, 0.5);
                cr.Paint();
            }

            base.OnSnapshot(snapshot, widget, background_area, cell_area, flags);
        }

        private void CellRendererText_EditingCanceled(object? o, EventArgs args)
        {
            owner.OnEditingFinished();
        }

        private void CellRendererText_Edited(object o, EditedArgs args)
        {
            owner.OnEditingFinished();
            var path = new TreePath(args.Path);
            var model = ((Gtk.TreeView)owner.tree).Model;
            model.GetIter(out var iter, path);
            var cell = model.GetValue(iter, 0);
            if (cell is GridEntry val)
            {
                var oldvalue = val.Value;
                owner.InPropertySet = true;
                try
                {
                    if (path.Depth == 1)
                    {
                        if (val.PropertyInfo?.CanWrite ?? false)
                        {
                            if (val.ValueType?.IsEnum ?? false)
                                val.PropertyInfo.SetValue(val.OwnerObj, Enum.Parse(val.PropertyInfo.PropertyType, args.NewText));
                            else
                                val.PropertyInfo.SetValue(val.OwnerObj, Convert.ChangeType(args.NewText, val.PropertyInfo.PropertyType));
                        }
                    }
                    else
                    {
                        if (val.ValueType?.IsEnum ?? false)
                        {
                            if (val.PropertyInfo?.CanWrite ?? false)
                            {
                                if (val.ValueType.IsEnum)
                                    val.PropertyInfo.SetValue(val.OwnerObj, Enum.Parse(val.PropertyInfo.PropertyType, args.NewText));
                                else
                                    val.PropertyInfo.SetValue(val.OwnerObj, Convert.ChangeType(args.NewText, val.PropertyInfo.PropertyType));
                            }
                        }
                        else
                        {
                            if (model.IterParent(out var piter, iter))
                            {   // get parent object
                                var pcell = model.GetValue(piter, 0);
                                if (pcell is GridEntry pval)
                                {
                                    if (val.PropertyInfo?.CanWrite ?? false)
                                    {
                                        if (val.ValueType?.IsEnum ?? false)
                                            val.PropertyInfo.SetValue(val.OwnerObj, Enum.Parse(val.PropertyInfo.PropertyType, args.NewText));
                                        else
                                            val.PropertyInfo.SetValue(val.OwnerObj, Convert.ChangeType(args.NewText, val.PropertyInfo.PropertyType));

                                        // modify parent object value
                                        if (pval.PropertyInfo?.CanWrite ?? false)
                                        {
                                            pval.PropertyInfo.SetValue(pval.OwnerObj, val.OwnerObj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.GtkLogger.Debug($"Unexpected exception in CellRendererText_Edited", ex);
                }
                finally
                {
                    // The set is over before anyone is told about it, so a PropertyValueChanged handler that
                    // asks PropertyGrid.InPropertySet sees false - the same order WinForms reports.
                    owner.InPropertySet = false;
                }
                owner.PropertyValueChanged?.Invoke(o, new PropertyValueChangedEventArgs(val, oldvalue));
            }
        }

        private void CellRendererText_EditingStarted(object o, EditingStartedArgs args)
        {
            owner.OnEditingStarted(args.Editable);
            model.Clear();
            if (_value?.ValueType != null)
            {
                if (_value.ValueType.IsEnum)
                {
                    var keys = Enum.GetNames(_value.ValueType);
                    foreach (var key in keys)
                    {
                        model.AppendValues(key);
                    }
                }
                else if (_value.ValueType.Equals(typeof(bool)))
                {
                    model.AppendValues("False");
                    model.AppendValues("True");
                }
            }
        }
    }
}