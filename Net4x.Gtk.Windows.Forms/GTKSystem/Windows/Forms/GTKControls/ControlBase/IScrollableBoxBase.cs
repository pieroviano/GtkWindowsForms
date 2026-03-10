using Gtk;

namespace GtkSystem.Windows.Forms.GtkControls.ControlBase
{
    public interface IScrollableBoxBase
    {
        event System.Windows.Forms.ScrollEventHandler Scroll;

        /// <summary>
        /// Gets or sets a value indicating whether automatic scroll.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if automatic scroll; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool AutoScroll { get; set; }

        /// <summary>
        /// Gets the child.
        /// </summary>
        /// <value>
        /// The child.
        /// </value>
        Widget Child { get; }

        /// <summary>
        /// Gets or sets the height request.
        /// </summary>
        /// <value>
        /// The height request.
        /// </value>
        int HeightRequest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether h scroll.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if h scroll; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool HScroll { get; set; }

        /// <summary>
        /// Gets or sets the override.
        /// </summary>
        /// <value>
        /// The override.
        /// </value>
        GtkControlOverride Override { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether v scroll.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if v scroll; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        bool VScroll { get; set; }

        /// <summary>
        /// Gets or sets the width request.
        /// </summary>
        /// <value>
        /// The width request.
        /// </value>
        int WidthRequest { get; set; }

        void Add(Widget child);

        /// <summary>
        /// Adds the class.
        /// </summary>
        /// <param name="cssClass">
        /// The CSS class.
        /// </param>
        void AddClass(string cssClass);

    }
}
