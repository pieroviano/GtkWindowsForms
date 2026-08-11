using Gtk;

namespace System.Windows.Forms
{

    public class StripMenuItem : Gtk.Box, IToolMenuItem
    {

        /// <summary>Gtk 4: widgets are visible by default; ShowAll maps to Show().</summary>
        public void ShowAll() => Show();

        public Gtk.CheckButton checkButton = new Gtk.CheckButton() { Visible = true }; // Gtk4: CheckButton has no Relief

        // Gtk4: a CheckButton renders EITHER its Label OR its Child, and assigning Child clears any prior
        // label. The previous code set `checkButton.Child = new Gtk.Image(...)` for an item's icon, which
        // wiped out the text - menu items with an image showed no caption (the "scattered icons, no labels"
        // Gtk4 regression). The button now hosts a small [icon][label] box so both show together, and the
        // check-indicator square is collapsed away by the .MenuItemButton / .MenuBarItem CSS (Gtk3 achieved
        // the same flat look with checkbutton.DrawIndicator = false, which Gtk4 removed).
        private readonly Gtk.Box contentBox = new Gtk.Box(Gtk.Orientation.Horizontal, 4) { Visible = true };

        private readonly Gtk.Image iconImage = new Gtk.Image() { Visible = false };

        private readonly Gtk.Label textLabel = new Gtk.Label() { Visible = false, Halign = Align.Start, Valign = Align.Center };

        private Gdk.Pixbuf _Image = null!;

        // Guards the Toggled->Activated bridge so that programmatic Checked/CheckState changes do not look
        // like a user click.
        private bool _suppressActivation;

        /// <summary>
        /// Raised when the user activates the item (pointer click or Space/Enter). Gtk4's GtkCheckButton
        /// derives from GtkWidget and has NO Clicked signal - only Toggled - so this is the click surface a
        /// ToolStripItem bridges to WinForms Click. Programmatic Checked/CheckState changes are suppressed.
        /// </summary>
        public event EventHandler? Activated;

        public StripMenuItem() : base()
        {
            base.Visible = true;
            checkButton.Halign = Align.Start;
            checkButton.Hexpand = true;
            checkButton.Vexpand = true;
            contentBox.Append(iconImage);
            contentBox.Append(textLabel);
            checkButton.Child = contentBox; // Gtk4: Box.Add -> Append; the button's content is the icon+label box
            base.Append(checkButton); // Gtk4: Box.Add -> Append
            checkButton.Toggled += CheckButton_Toggled;
        }

        private void CheckButton_Toggled(object? sender, EventArgs e)
        {
            if (_suppressActivation)
            {
                return;
            }

            // WinForms: a menu item's Checked does NOT change when the user clicks it (CheckOnClick defaults
            // false; checkable items - e.g. the demo's cMenu - are driven programmatically via Checked). Undo
            // the toggle GtkCheckButton just applied so clicking a command never leaves a stray checkmark or
            // a permanently "checked" item, then report the activation.
            SetActiveSuppressed(!checkButton.Active);
            Activated?.Invoke(this, EventArgs.Empty);
        }

        public bool Checked
        {
            // Gtk4 removed CheckButton.DrawIndicator; Checked reflects the toggle state (Active),
            // consistent with CheckState below.
            get => checkButton.Active;
            set => SetActiveSuppressed(value);
        }

        public CheckState CheckState
        {
            get => checkButton.Active == true ? CheckState.Checked : CheckState.Unchecked;
            set => SetActiveSuppressed(value == CheckState.Checked);
        }

        // Setting Active fires Toggled; when we do it in code (not a user click) that must not be reported as
        // an activation, or e.g. initializing a menu item's Checked would fire its Click handler.
        private void SetActiveSuppressed(bool active)
        {
            var previous = _suppressActivation;
            _suppressActivation = true;
            try
            {
                checkButton.Active = active;
            }
            finally
            {
                _suppressActivation = previous;
            }
        }

        public ToolStripItemDisplayStyle DisplayStyle
        {
            get;
            set;
        }

        public Gdk.Pixbuf Image
        {
            get => _Image;
            set
            {
                _Image = value;
                if (value == null)
                {
                    iconImage.Visible = false;
                }
                else
                {
                    // Update the persistent icon widget instead of replacing the button child, so the
                    // caption Label sitting next to it survives.
                    iconImage.Pixbuf = value;
                    iconImage.Visible = true;
                }
            }
        }

        public Gtk.Widget MenuItem { get => this; }

        public string Text
        {
            get => textLabel.Text;
            set
            {
                textLabel.Text = value ?? string.Empty;
                textLabel.Visible = !string.IsNullOrEmpty(value);
            }
        }

        public Gtk.Widget ToolItem { get; } = null!;

        /// <summary>
        /// Lays the icon and caption out according to a WinForms <see cref="TextImageRelation" />: the
        /// *AboveText / TextAbove* relations stack them vertically (the Gtk3 top-level "icon above text"
        /// look), the *BeforeText / TextBefore* relations keep them side by side; the order follows which of
        /// text/image comes first.
        /// </summary>
        public void SetTextImageRelation(TextImageRelation relation)
        {
            var vertical = relation == TextImageRelation.ImageAboveText || relation == TextImageRelation.TextAboveImage;
            var textFirst = relation == TextImageRelation.TextAboveImage || relation == TextImageRelation.TextBeforeImage;

            contentBox.Orientation = vertical ? Gtk.Orientation.Vertical : Gtk.Orientation.Horizontal;

            // Gtk4 Box has no reorder-child; remove and re-append in the wanted order.
            contentBox.Remove(iconImage);
            contentBox.Remove(textLabel);
            if (textFirst)
            {
                contentBox.Append(textLabel);
                contentBox.Append(iconImage);
            }
            else
            {
                contentBox.Append(iconImage);
                contentBox.Append(textLabel);
            }

            var align = vertical ? Align.Center : Align.Start;
            contentBox.Halign = align;
            iconImage.Halign = align;
            textLabel.Halign = align;
        }

        /// <summary>
        /// Configures this item as a top-level menu-bar entry: it packs tightly at the start of the bar
        /// rather than stretching across it. (checkButton.Hexpand propagates up and would spread the items
        /// evenly, which is why Menu 1 / Menu 2 drifted apart on Gtk4.) A right-aligned item opts back into
        /// expansion via <see cref="ToolStripItem.Alignment" /> = Right.
        /// </summary>
        public void ConfigureAsMenuBarItem()
        {
            checkButton.Hexpand = false;
            checkButton.Vexpand = false;
            this.Hexpand = false;
            this.Halign = Align.Start;
        }

        // Gtk4 removed Gtk.Menu/Gtk.MenuItem.Submenu; a drop-down is a Gtk.Popover parented to the item and
        // holding its child items in a vertical box. Created lazily the first time a sub-item is added.
        private Gtk.Popover? _dropDown;

        private Gtk.Box? _dropDownBox;

        /// <summary>
        /// Returns the vertical content box of this item's drop-down popover, creating and anchoring the
        /// popover to the item's clickable surface on first use. Sub-items are appended to the returned box.
        /// </summary>
        public Gtk.Box EnsureDropDownContent()
        {
            if (_dropDown == null)
            {
                _dropDownBox = new Gtk.Box(Gtk.Orientation.Vertical, 0) { Visible = true };
                _dropDown = new Gtk.Popover
                {
                    Autohide = true,
                    Position = Gtk.PositionType.Bottom,
                    Child = _dropDownBox,
                    HasArrow = false,
                    // A bottom popover centres on its parent by default, so a drop-down wider than a narrow
                    // menu item spilled to the left. Halign.Start left-aligns the popover's edge with the
                    // item's left edge, matching WinForms/Gtk3.
                    Halign = Gtk.Align.Start,
                };
                // Gtk4: gtk_widget_set_parent anchors the popover to the item's button; Popup() then shows it
                // relative to that widget.
                _dropDown.Parent = checkButton;
            }

            return _dropDownBox!;
        }

        /// <summary>Whether this item has a populated drop-down to show.</summary>
        public bool HasDropDown => _dropDownBox != null && _dropDownBox.FirstChild != null;

        /// <summary>The horizontal alignment of the drop-down popover relative to this item (for tests).</summary>
        public Gtk.Align DropDownAlign => _dropDown?.Halign ?? Gtk.Align.Fill;

        /// <summary>
        /// Pops the drop-down open (no-op if the item has no drop-down, or is not yet on screen - Popup on an
        /// unmapped popover would abort in GTK).
        /// </summary>
        public void ShowDropDown()
        {
            if (_dropDown != null && checkButton.IsMapped)
            {
                _dropDown.Popup();
            }
        }

        /// <summary>Closes the drop-down if it is open.</summary>
        public void HideDropDown() => _dropDown?.Popdown();

        /// <summary>
        /// Dismisses the drop-down/context popover(s) this item lives in - selecting a WinForms menu command
        /// closes the whole open menu chain. Walks the parent chain (item -> content box -> popover, and on
        /// up through any parent menus) and pops each popover down.
        /// </summary>
        public void CloseContainingPopover()
        {
            for (Gtk.Widget? w = Parent; w != null; w = w.Parent)
            {
                if (w is Gtk.Popover popover)
                {
                    popover.Popdown();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            _Image?.Dispose();
            checkButton.Dispose();
            if (_dropDown != null)
            {
                // A popover parented via set_parent must be explicitly unparented before disposal.
                _dropDown.Unparent();
                _dropDown.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
