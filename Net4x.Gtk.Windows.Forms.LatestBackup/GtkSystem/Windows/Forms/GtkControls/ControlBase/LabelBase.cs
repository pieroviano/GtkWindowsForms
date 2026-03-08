namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    /// <summary>
    /// LabelBase is a base class for GTK label controls, providing common functionality and properties for derived classes. It implements the IControlGtk interface, allowing it to integrate with the GTK system and provide consistent behavior across different types of label controls. The LabelBase class includes features such as text alignment, word wrapping, and custom drawing capabilities.
    /// </summary>
    /// <seealso cref="Gtk.Label" />
    /// <seealso cref="GtkSystem.Windows.Forms.GtkControls.ControlBase.IControlGtk" />
    public sealed class LabelBase : Gtk.Label, IControlGtk
    {
        public LabelBase() : base()
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("Label");
            this.Xalign = 0.0f;
            this.Yalign = 0.0f;
            this.Valign = Gtk.Align.Start;
            this.Halign = Gtk.Align.Start;
            this.Wrap = true;
            this.LineWrap = true;
            this.LineWrapMode = Pango.WrapMode.WordChar;
        }

        public LabelBase(string text) : base(text)
        {
            this.Override = new GtkControlOverride(this);
            this.Override.AddClass("Label");
            this.Xalign = 0.0f;
            this.Yalign = 0.0f;
            this.Valign = Gtk.Align.Start;
            this.Halign = Gtk.Align.Start;
            this.Wrap = true;
            this.LineWrap = true;
            this.LineWrapMode = Pango.WrapMode.WordChar;
        }

        public GtkControlOverride Override { get; set; }

        protected override bool OnDrawn(Cairo.Context cr)
        {
            Gdk.Rectangle rec = new Gdk.Rectangle(0, 0, this.AllocatedWidth, this.AllocatedHeight);
            Override.OnDrawnBackground(cr, rec);
            Override.OnPaint(cr, rec);

            return base.OnDrawn(cr);
        }

        protected override void OnShown()
        {
            Override.OnAddClass();
            base.OnShown();
        }

    }
}
