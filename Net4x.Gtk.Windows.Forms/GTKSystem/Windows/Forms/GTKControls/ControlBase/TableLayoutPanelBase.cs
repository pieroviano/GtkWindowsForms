namespace GtkSystem.Windows.Forms.GtkControls.ControlBase;

/// <summary>
/// TableLayoutPanelBase class is a base class for GTK table layout panel controls, providing common functionality and properties for derived classes. It inherits from ScrollableBoxBase and implements the IControlGtk interface. The class includes a constructor that initializes the table layout panel and sets various properties such as border width, expansion, and scrollbar policies. It also provides an Override property of type GtkControlOverride, which allows for custom styling and behavior of the control. By using TableLayoutPanelBase, developers can easily create custom table layout panel controls that are compatible with the GTK system and provide consistent behavior across different platforms.
/// </summary>
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.ScrollableBoxBase" />
/// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
public sealed class TableLayoutPanelBase : ScrollableBoxBase, IControlGtk
{

    /// <summary>
    /// The grid
    /// </summary>
    public Gtk.Grid grid = new Gtk.Grid();

    /// <summary>
    /// Initializes a new instance of the <see cref="TableLayoutPanelBase"/> class.
    /// </summary>
    public TableLayoutPanelBase() : base()
    {
        this.Override = new GtkControlOverride(this);
        this.Override.AddClass("TableLayoutPanel");
        grid.RowHomogeneous = false;
        grid.ColumnHomogeneous = false;
        grid.BaselineRow = 0;
        grid.ColumnSpacing = 0;
        grid.RowSpacing = 0;
        grid.BorderWidth = 0;
        grid.Halign = Gtk.Align.Fill;
        grid.Valign = Gtk.Align.Fill;
        this.BorderWidth = 0;
        this.Vexpand = false;
        this.Hexpand = false;
        this.HscrollbarPolicy = Gtk.PolicyType.External;
        this.VscrollbarPolicy = Gtk.PolicyType.External;

        this.Add(this.grid);
    }

    /// <summary>
    /// Called when the control is drawn.
    /// </summary>
    /// <param name="cr">The Cairo context.</param>
    /// <returns></returns>
    protected override bool OnDrawn(Cairo.Context cr)
    {
        Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
        Override.OnPaint(cr, rec);

        return base.OnDrawn(cr);
    }

    /// <summary>
    /// Called when the control is shown.
    /// </summary>
    protected override void OnShown()
    {
        Override.OnAddClass();
        base.OnShown();
    }

}
