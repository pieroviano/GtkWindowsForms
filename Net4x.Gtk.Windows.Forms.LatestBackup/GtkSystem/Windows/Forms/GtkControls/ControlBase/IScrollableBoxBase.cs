using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public interface IScrollableBoxBase
    {
        event System.Windows.Forms.ScrollEventHandler Scroll;
        bool AutoScroll { get; set; }

        Widget Child { get; }

        int HeightRequest { get; set; }

        bool HScroll { get; set; }

        GtkControlOverride Override { get; set; }

        bool VScroll { get; set; }

        int WidthRequest { get; set; }
        void Add(Widget child);

        void AddClass(string cssClass);
    }
}
