/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
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
            otherBox.PackStart(img, false, true, 1);
            otherBox.ReorderChild(img, 0);
        }

        var expander = new CellRendererExpander(this)
        {
            CellBackgroundRgba = new RGBA() { Alpha = 0.6, Red = 0.9, Green = 0.9, Blue = 0.9 }
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
            var attri = propertyInfo.GetCustomAttributes(typeof(BrowsableAttribute));
            if (attri != null && attri.Count() > 0)
            {
                //BrowsableAttribute
                var attribute = (BrowsableAttribute)attri.FirstOrDefault();
                if (attribute.Browsable == false)
                {
                    continue;
                }
            }
            else
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
                var value0 = new GridEntry(null, GridItemType.Category, 0, g.Key, g.Key, g.Key) { Level = 0, Editable = false };
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
            var attri = property.GetCustomAttributes(typeof(BrowsableAttribute));
            if (attri != null && attri.Count() > 0)
            {
                //BrowsableAttribute
                var attribute = (BrowsableAttribute)attri.FirstOrDefault();
                if (attribute.Browsable == false)
                {
                    return null;
                }
            }
            else if (parent == null)
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
                var value1 = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, obj, description) { Editable = property.CanWrite, ValueType = property.PropertyType, PropertyInfo = property };
                StoreValue(parent, value1);
                return value1;
            }
            else if (property.PropertyType.IsValueType)
            {
                if (property.PropertyType.IsPrimitive == false)
                { // only primitive types can be edited
                    var value1 = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, obj, description) { ValueType = property.PropertyType, PropertyInfo = property };
                    var node1 = StoreValue(parent, value1);
                    var val = property.GetValue(obj);
                    if (property.CanWrite)
                        GetPropertyInfos(node1, value1, val);
                    return value1;
                }
                else
                {
                    var value1 = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, obj, description) { Editable = property.CanWrite, ValueType = property.PropertyType, PropertyInfo = property };
                    var node1 = StoreValue(parent, value1);
                    return value1;
                }
            }
            else
            {
                var value1 = new GridEntry(parentitem, GridItemType.Property, 1, property.Name, obj, description) { Editable = false, ValueType = property.PropertyType, PropertyInfo = property };
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

        protected override void OnRender(Context cr, Widget widget, Gdk.Rectangle background_area, Gdk.Rectangle cell_area, CellRendererState flags)
        {
            if (this.IsExpander)
            {
                if (_value?.Level == 0)
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
                owner.PropertyValueChanged?.Invoke(o, new PropertyValueChangedEventArgs(val, oldvalue));
            }
        }

        private void CellRendererText_EditingStarted(object o, EditingStartedArgs args)
        {
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