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

        // Gtk3 got the "this item opens a submenu" triangle for free from GtkMenuItem's theming; Gtk4 has no
        // menu-item widget, so the indicator is drawn here. A Label rather than a themed Gtk.Image because a
        // missing icon name would silently render nothing, and the arrow is load-bearing information.
        private readonly Gtk.Label submenuArrow = new Gtk.Label("▸")
        {
            Visible = false,
            Halign = Align.End,
            Valign = Align.Center,
            Hexpand = true,
        };

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
            contentBox.Append(submenuArrow);
            submenuArrow.StyleContext.AddClass("SubmenuArrow");
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
                    // Gtk3's GtkImage drew a pixbuf at its natural size. Gtk4's renders it at the theme's
                    // icon size (16px) whatever the pixbuf measures, so scaling the pixbuf alone had no
                    // visible effect and every menu icon came out smaller than on Gtk3. PixelSize is what
                    // makes the widget honour the size we scaled to.
                    iconImage.PixelSize = Math.Max(value.Width, value.Height);
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
            contentBox.Remove(submenuArrow);
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

            // The submenu indicator always trails the caption, whatever relation icon and text are in.
            contentBox.Append(submenuArrow);

            var align = vertical ? Align.Center : Align.Start;
            contentBox.Halign = align;
            iconImage.Halign = align;
            textLabel.Halign = align;

            // Re-assert the indicator layout, which needs the row to span the item's full width.
            ApplyDropDownPlacement();
        }

        /// <summary>
        /// Configures this item as a top-level menu-bar entry: it packs tightly at the start of the bar
        /// rather than stretching across it. (checkButton.Hexpand propagates up and would spread the items
        /// evenly, which is why Menu 1 / Menu 2 drifted apart on Gtk4.) A right-aligned item opts back into
        /// expansion via <see cref="ToolStripItem.Alignment" /> = Right.
        /// </summary>
        public void ConfigureAsMenuBarItem()
        {
            _isMenuBarItem = true;
            checkButton.Hexpand = false;
            checkButton.Vexpand = false;
            this.Hexpand = false;
            this.Halign = Align.Start;

            // A drop-down may already have been built (the designer commonly fills DropDownItems before
            // adding the item to the MenuStrip), so re-place it now that we know this is a bar entry.
            ApplyDropDownPlacement();
        }

        /// <summary>
        /// Whether this item is a top-level entry of a menu bar, as opposed to an entry inside a drop-down
        /// or context menu. Set by <see cref="ConfigureAsMenuBarItem" /> when the item is added to a
        /// <see cref="MenuStrip" />.
        /// </summary>
        private bool _isMenuBarItem;

        /// <summary>
        /// Configures this item as an entry inside a drop-down or context menu, where rows span the whole
        /// width of the menu.
        /// </summary>
        /// <remarks>
        /// The clickable surface is left-hugging by default, which suits a menu bar but not a menu row. In a
        /// drop-down it has two visible consequences: only the caption highlights on hover instead of the
        /// whole row, and - because a popover anchors to the widget it is attached to - a submenu opened to
        /// the right started at the end of the caption rather than at the menu's edge, overlapping the menu
        /// it came from.
        /// </remarks>
        public void ConfigureAsDropDownItem()
        {
            checkButton.Halign = Align.Fill;
            checkButton.Hexpand = true;
            this.Halign = Align.Fill;
            this.Hexpand = true;
        }

        /// <summary>
        /// Positions this item's drop-down the way WinForms and Gtk3 do, and shows or hides the submenu
        /// indicator to match.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A menu-bar entry drops its menu <em>downwards</em>, left-aligned with the entry. Every other item
        /// that owns a drop-down is a submenu parent, and its menu must cascade to the <em>right</em> with
        /// its top edge level with the item.
        /// </para>
        /// <para>
        /// Gtk3 never needed this: <c>GtkMenuItem.Submenu</c> placed cascading menus itself. Gtk4 removed
        /// the menu widgets, and a <see cref="Gtk.Popover" /> defaults to dropping downwards - so a submenu
        /// opened on top of the menu it came from, hiding the items below its parent.
        /// </para>
        /// </remarks>
        private void ApplyDropDownPlacement()
        {
            // Only a submenu parent gets an arrow; a menu-bar entry never shows one.
            submenuArrow.Visible = _dropDown != null && !_isMenuBarItem;

            if (submenuArrow.Visible)
            {
                // The arrow sits at the item's trailing edge, so the content row has to span the whole item
                // rather than hugging its caption.
                contentBox.Hexpand = true;
                contentBox.Halign = Align.Fill;
            }

            if (_dropDown == null)
            {
                return;
            }

            if (_isMenuBarItem)
            {
                _dropDown.Position = Gtk.PositionType.Bottom;
                // A bottom popover centres on its parent by default, so a drop-down wider than a narrow menu
                // item spilled to the left. Start left-aligns the popover with the item's left edge.
                _dropDown.Halign = Align.Start;
                _dropDown.Valign = Align.Fill;
            }
            else
            {
                _dropDown.Position = Gtk.PositionType.Right;
                _dropDown.Halign = Align.Fill;
                // Level the submenu's top edge with its parent item instead of centring it on the item.
                _dropDown.Valign = Align.Start;
            }
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
                    Child = _dropDownBox,
                    HasArrow = false,
                };

                // Direction and alignment depend on whether this item is a menu-bar entry or a submenu
                // parent - see ApplyDropDownPlacement.
                ApplyDropDownPlacement();
            }

            return _dropDownBox!;
        }

        /// <summary>Whether this item has a populated drop-down to show.</summary>
        public bool HasDropDown => _dropDownBox != null && _dropDownBox.FirstChild != null;

        /// <summary>The horizontal alignment of the drop-down popover relative to this item (for tests).</summary>
        public Gtk.Align DropDownAlign => _dropDown?.Halign ?? Gtk.Align.Fill;

        /// <summary>The vertical alignment of the drop-down popover relative to this item (for tests).</summary>
        public Gtk.Align DropDownValign => _dropDown?.Valign ?? Gtk.Align.Fill;

        /// <summary>
        /// The side of this item its drop-down opens on: <see cref="Gtk.PositionType.Bottom" /> for a
        /// menu-bar entry, <see cref="Gtk.PositionType.Right" /> for a submenu parent.
        /// </summary>
        public Gtk.PositionType DropDownPosition => _dropDown?.Position ?? Gtk.PositionType.Bottom;

        /// <summary>Whether the "opens a submenu" arrow is currently shown on this item.</summary>
        public bool ShowsSubmenuArrow => submenuArrow.Visible;

        /// <summary>The submenu indicator widget, so its position in the content row can be asserted.</summary>
        public Gtk.Widget SubmenuArrowWidget => submenuArrow;

        /// <summary>
        /// The size, in pixels, the icon is actually rendered at - as opposed to the size of the pixbuf
        /// behind it, which Gtk4 ignores unless the widget is told otherwise.
        /// </summary>
        public int IconPixelSize => iconImage.PixelSize;

        /// <summary>
        /// Pops the drop-down open (no-op if the item has no drop-down, or is not yet on screen - Popup on an
        /// unmapped popover would abort in GTK).
        /// </summary>
        public void ShowDropDown()
        {
            if (_dropDown != null && checkButton.IsMapped)
            {
                // Gtk4: anchor the popover to the item's button (gtk_widget_set_parent) only now that the item
                // is realized and on screen. Parenting it at construction time - while the button is still
                // unrealized and not in a window - crashes the process.
                if (_dropDown.Parent == null)
                {
                    _dropDown.Parent = checkButton;
                    _dropDown.Closed += (_, _) => SetDropDownOpenAppearance(false);
                }

                _dropDown.Popup();
                SetDropDownOpenAppearance(true);
            }
        }

        /// <summary>
        /// Marks this item as the open one while its drop-down is showing, so it reads as selected.
        /// </summary>
        /// <param name="open">Whether the drop-down is open.</param>
        /// <remarks>
        /// Gtk3 needed nothing here: a <c>GtkMenuItem</c> whose submenu is open puts itself in the selected
        /// state and the theme paints it. Gtk4 has no menu-item widget - a drop-down is a plain popover
        /// anchored to a button - so the item stayed unpainted and it was impossible to tell which entry the
        /// open submenu belonged to. Gtk4's own <c>GtkPopoverMenu</c> highlights the row the same way, so
        /// this restores standard behaviour rather than emulating a Gtk3 quirk.
        /// </remarks>
        public void SetDropDownOpenAppearance(bool open)
        {
            if (open)
            {
                checkButton.StyleContext.AddClass(DropDownOpenClass);
                contentBox.StyleContext.AddClass(DropDownOpenRowClass);
            }
            else
            {
                checkButton.StyleContext.RemoveClass(DropDownOpenClass);
                contentBox.StyleContext.RemoveClass(DropDownOpenRowClass);
            }
        }

        /// <summary>
        /// The style class carrying the selected <em>background</em>, applied to the item's button.
        /// </summary>
        public const string DropDownOpenClass = "MenuItemOpen";

        /// <summary>
        /// The style class carrying the selected <em>foreground</em>, applied to the caption row only.
        /// </summary>
        /// <remarks>
        /// The two are deliberately separate. The drop-down popover is parented to the button (that is what
        /// positions it), so anything inheritable set on the button cascades into the menu it opens - a
        /// selected foreground colour there repainted every caption in the drop-down white on white and the
        /// menu appeared empty. Background does not inherit, so it can stay on the button; the foreground
        /// goes on the caption row, which is not an ancestor of the popover.
        /// </remarks>
        public const string DropDownOpenRowClass = "MenuItemOpenRow";

        /// <summary>Whether this item is currently painted as the open one (for tests).</summary>
        public bool IsDropDownOpenStyled => checkButton.StyleContext.HasClass(DropDownOpenClass);

        /// <summary>
        /// The widget the drop-down popover is parented to. Exposed so tests can assert that nothing
        /// inheritable is styled on it.
        /// </summary>
        public Gtk.Widget DropDownHost => checkButton;

        /// <summary>Closes the drop-down if it is open.</summary>
        public void HideDropDown()
        {
            _dropDown?.Popdown();
            SetDropDownOpenAppearance(false);
        }

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
                // A popover parented via set_parent must be explicitly unparented before disposal; if it was
                // never shown it has no parent, and unparenting it then would trip a GTK-CRITICAL.
                if (_dropDown.Parent != null)
                {
                    _dropDown.Unparent();
                }

                _dropDown.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
