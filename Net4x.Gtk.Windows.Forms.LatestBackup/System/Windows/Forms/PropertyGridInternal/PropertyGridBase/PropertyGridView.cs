/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.ComponentModel;
using System.Reflection;
using Atk;
using Cairo;
using Gdk;
using GLib;
using Gtk;

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed partial class PropertyGridView
{

    public Gtk.TreeStore store = new Gtk.TreeStore(typeof(GridEntry));

    public Gtk.TreeView tree = new Gtk.TreeView();

    private object _selectedObject;

    private GridEntry oldSelectedPropertyItem;

    public event PropertyValueChangedEventHandler PropertyValueChanged;

    public event SelectedGridItemChangedEventHandler SelectedGridItemChanged;

    public PropertyGridView(PropertyGrid propertyGrid)
    {
        OwnerGrid = propertyGrid;
        tree.ShowExpanders = false;
        tree.EnableTreeLines = false;
        tree.EnableGridLines = TreeViewGridLines.Horizontal;
        var iconTheme = new IconTheme();
        var treeViewColumn = new Gtk.TreeViewColumn
        {
            FixedWidth = 30,
            Resizable = false,
            Clickable = false,
            SortIndicator = false,
            Reorderable = false
        };
        var button = treeViewColumn.Button as Gtk.Button;
        if (button.Child is Gtk.Box box1)
        {
            var img = new Gtk.Image(this.GetType().Assembly, "Resources.System.PBCategory.ico")
            {
                Visible = true
            };
            box1.PackStart(img, false, true, 1);
            box1.ReorderChild(img, 0);
        }

        var expander = new CellRendererExpander(this)
        {
            CellBackgroundRgba = new RGBA() { Alpha = 0.6, Red = 0.9, Green = 0.9, Blue = 0.9 }
        };
        treeViewColumn.PackStart(expander, false);
        treeViewColumn.AddAttribute(expander, "icon", 0);

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
        if (expanderColumnButton.Child is Gtk.Box box)
        {
            var img = new Gtk.Image(this.GetType().Assembly, "Resources.System.PBAlpha.ico")
            {
                Visible = true
            };
            box.PackStart(img, false, true, 1);
            box.ReorderChild(img, 0);
            box.SetChildPacking(box.Children[2], false, true, 0, PackType.End);
        }
        CellRendererText names = new CellRendererProperty(this)
        {
            Editable = false
        };
        expanderColumn.SortColumnId = 0;
        expanderColumn.PackStart(names, false);
        expanderColumn.AddAttribute(names, "otext", 0);

        var column = new Gtk.TreeViewColumn
        {
            MinWidth = 10,
            Resizable = false,
            Clickable = false,
            Title = " ",
            SortIndicator = false,
            Reorderable = false
        };
        column.Button.Name = "value";
        Gtk.CellRendererText values = new CellRendererProperty(this)
        {
            Editable = true
        };
        column.PackStart(values, false);
        column.AddAttribute(values, "ovalue", 0);

        tree.AppendColumn(treeViewColumn);
        tree.AppendColumn(expanderColumn);
        tree.AppendColumn(column);
        tree.ExpanderColumn = expanderColumn;

        tree.LevelIndentation = 15;
        tree.Selection.Mode = Gtk.SelectionMode.Single;
        tree.ActivateOnSingleClick = true;
        tree.RowActivated += Tree_RowActivated;
        tree.Selection.Changed += Selection_Changed;

        store.SetSortFunc(0, new TreeIterCompareFunc((model, iter1, iter2) =>
        {
            var val1 = model.GetValue(iter1, 0) as GridEntry;
            var val2 = model.GetValue(iter2, 0) as GridEntry;
            return val1.Value.ToString().CompareTo(val2.Value.ToString());
        }));

        tree.Model = store;
    }

    public PropertyGrid OwnerGrid
    {
        get;
        private set;
    }

    public void LoadPropertyInfo(object entryobj)
    {
        _selectedObject = entryobj;
        var categorys = new List<GridEntry>();
        var propertyInfos = entryobj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
        foreach (var propertyInfo in propertyInfos)
        {
            var attri = propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute));
            if (attri != null && attri.Count() > 0)
            {
                //BrowsableAttribute
                var attribute = (BrowsableAttribute)attri.FirstOrDefault();
                if (!attribute.Browsable)
                {
                    continue;
                }
            }
            var category = "【 uncategorized group】";
            var description = "";

            var attributes = propertyInfo.GetCustomAttributes(typeof(CategoryAttribute));
            if (attributes != null && attributes.Count() > 0)
            {
                var attribute = (CategoryAttribute)attributes.FirstOrDefault();
                category = attribute.Category;
            }
            var customAttributes = propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute));
            if (customAttributes != null && customAttributes.Count() > 0)
            {
                var attribute = (DescriptionAttribute)customAttributes.FirstOrDefault();
                description = attribute.Description;
            }

            var propertyItem = new GridEntry(null, GridItemType.Category, 0, category, category, description)
            {
                PropertyInfo = propertyInfo
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
                var value = new GridEntry(null, GridItemType.Category, 0, g.Key, g.Key, g.Key) { Level = 0, Editable = false };
                var iter = store.AppendValues(value);
                foreach (var m in g)
                {
                    GetPropertyInfo(iter, value, m.PropertyInfo, entryobj);
                }
            }
        }
        tree.ExpandAll();
    }

    private bool GetPropertyInfo(Gtk.TreeIter? parent, GridEntry parentitem, PropertyInfo property, object obj)
    {
        if (property.CanRead)
        {
            var attri = property.GetCustomAttributes(typeof(BrowsableAttribute));
            if (attri != null && attri.Count() > 0)
            {
                //BrowsableAttribute
                var attribute = (BrowsableAttribute)attri.FirstOrDefault();
                if (!attribute.Browsable)
                {
                    return false;
                }
            }
            var description = "";
            var attributes = property.GetCustomAttributes(typeof(DescriptionAttribute));
            if (attributes != null && attributes.Count() > 0)
            {
                var attribute = (DescriptionAttribute)attributes.FirstOrDefault();
                description = attribute.Description;
            }
            if (property.PropertyType.IsEnum)
            {
                var val = property.GetValue(obj);
                var value = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, val, description) { Editable = property.CanWrite, ValueType = property.PropertyType };
                StoreValue(parent, value);
            }
            else if (property.PropertyType.IsValueType)
            {
                var val = property.GetValue(obj);
                if (!property.PropertyType.IsPrimitive)
                { // only primitive types can be edited
                    var value = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, val, description) { ValueType = property.PropertyType };
                    var storeValue = StoreValue(parent, value);
                    GetPropertyInfos(storeValue, value, val);
                }
                else
                {
                    var value = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, val, description) { Editable = property.CanWrite, ValueType = property.PropertyType };
                    var storeValue = StoreValue(parent, value);
                }
            }
            else if (property.PropertyType.IsArray)
            {
                var val = property.GetValue(obj);
                var value = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, val, description) { Editable = false, ValueType = property.PropertyType };
                var storeValue = StoreValue(parent, value);
            }
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GetPropertyInfos(Gtk.TreeIter? parent, GridEntry parentitem, object obj)
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
        if (tree.Selection.GetSelected(out var model, out var oiter))
        {
            var propertyItem = store.GetValue(oiter, 0) as GridEntry;
            SelectedGridItemChanged?.Invoke(sender, new SelectedGridItemChangedEventArgs(oldSelectedPropertyItem, propertyItem));
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

    private void Tree_RowActivated(object o, RowActivatedArgs args)
    {
        Console.WriteLine(args.Path);
        if (store.GetIter(out var iter, args.Path))
        {
            if (store.GetValue(iter, 0) is GridEntry ov)
            {
                OwnerGrid.self.ShowDescription(ov.Label, ov.Description);
            }
        }
    }

    public class CellRendererExpander : Gtk.CellRendererToggle
    {
        readonly Pixbuf godownixbuf = new Pixbuf(typeof(CellRendererExpander).Assembly, "Resources.System.go-down-symbolic.png");

        readonly Pixbuf gonextpixbuf = new Pixbuf(typeof(CellRendererExpander).Assembly, "Resources.System.go-next-symbolic.png");

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
                tree.CollapseRow(new TreePath(path));
            else
                tree.ExpandToPath(new TreePath(path));

            return re;
        }

        protected override void OnRender(Context cr, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
        {
            if (this.IsExpander)
            {
                cr.Save();
                if (this.IsExpanded)
                    Gdk.CairoHelper.SetSourcePixbuf(cr, godownixbuf, background_area.Left + 10, background_area.Top + 5);
                else
                    Gdk.CairoHelper.SetSourcePixbuf(cr, gonextpixbuf, background_area.Left + 10, background_area.Top + 5);
                cr.Paint();
                cr.Restore();
            }
        }
    }

    public class CellRendererProperty : Gtk.CellRendererCombo
    {
        readonly Gtk.ListStore model = new Gtk.ListStore(typeof(string));

        readonly PropertyGridView owner;

        private GridEntry _value;

        public CellRendererProperty(PropertyGridView owner)
        {
            this.owner = owner;
            this.TextColumn = 0;
            this.Height = 20;
            this.SetFixedSize(50, 20);
            this.Model = model;
            this.EditingStarted += CellRendererText_EditingStarted;
            this.Edited += CellRendererText_Edited;
        }

        [Property("otext")]
        public GridEntry OText
        {
            set
            {
                _value = value;
                if (_value != null)
                {
                    base.Text = value.Label;
                    base.Editable = false;
                    base.Weight = value.Level == 0 ? 900 : 200;
                }
            }

            get { return _value; }
        }

        [Property("ovalue")]
        public GridEntry OValue
        {
            set
            {
                _value = value;
                if (_value != null)
                {
                    base.Text = value.Value?.ToString();
                    base.Editable = value.Editable;
                    base.Weight = value.Level == 0 ? 900 : 200;
                }
            }

            get { return _value; }
        }

        protected override void OnRender(Context cr, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
        {
            if (this.IsExpander)
            {
                if (_value.Level == 0)
                {
                    cell_area.X = 30;
                    background_area.X = 30;

                    cr.Save();
                    cr.SetSourceColor(new Cairo.Color() { A = 0.5, R = 0.9, G = 0.9, B = 0.9 });
                    cr.Paint();
                    cr.Restore();
                }
            }
            base.OnRender(cr, widget, background_area, cell_area, flags);
        }

        private void CellRendererText_Edited(object o, EditedArgs args)
        {
            var path = new TreePath(args.Path);
            var model = ((Gtk.TreeView)owner.tree).Model;
            model.GetIter(out var iter, path);
            var cell = model.GetValue(iter, 0);
            if (cell is GridEntry val)
            {
                var oldvalue = val.Value;
                try
                {
                    if (path.Depth <= 2)
                    {
                        var obj = owner._selectedObject;
                        var info = obj.GetType().GetProperty(val.Label);
                        var value = info.GetValue(obj);
                        if (info.CanWrite)
                        {
                            if (val.ValueType.IsEnum)
                                val.value = Enum.Parse(info.PropertyType, args.NewText);
                            else
                                val.value = Convert.ChangeType(args.NewText, info.PropertyType);
                            info.SetValue(obj, val.value);
                        }
                    }
                    else
                    {
                        if (val.ValueType.IsEnum)
                        {
                            var obj = owner._selectedObject;
                            var info = obj.GetType().GetProperty(val.Label);
                            if (info.CanWrite)
                            {
                                if (val.ValueType.IsEnum)
                                    val.value = Enum.Parse(info.PropertyType, args.NewText);
                                else
                                    val.value = Convert.ChangeType(args.NewText, info.PropertyType);
                                info.SetValue(obj, val.value);
                            }
                        }
                        else
                        {
                            if (model.IterParent(out var piter, iter))
                            {   // get parent object
                                var pcell = model.GetValue(piter, 0);
                                if (pcell is GridEntry pval)
                                {
                                    var obj = owner._selectedObject;
                                    var otype = obj.GetType();
                                    var pinfo = otype.GetProperty(pval.Label);
                                    var pvalue = pinfo.GetValue(obj);
                                    //从obj get current object，赋值
                                    var info = pvalue.GetType().GetProperty(val.Label);
                                    if (info.CanWrite)
                                    {
                                        if (val.ValueType.IsEnum)
                                            val.value = Enum.Parse(info.PropertyType, args.NewText);
                                        else
                                            val.value = Convert.ChangeType(args.NewText, info.PropertyType);
                                        info.SetValue(pvalue, val.value);
                                        pval.value = pvalue.ToString();
                                    }
                                    // modify parent object value
                                    if (pinfo.CanWrite)
                                    {
                                        pinfo.SetValue(obj, pvalue);
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
                owner.PropertyValueChanged?.Invoke(o, new PropertyValueChangedEventArgs(val, oldvalue));
            }
        }

        private void CellRendererText_EditingStarted(object o, EditingStartedArgs args)
        {
            model.Clear();
            if (_value.ValueType != null)
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