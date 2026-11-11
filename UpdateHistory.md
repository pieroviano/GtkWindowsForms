## Update History

  #### 2026/11/11
	1. Implemented Form.CenterToScreen and Form.CenterToParent, including screen-working-area
	   clamping, owner/parent fallback, and native placement on supported GTK backends. Centering
	   requested before the native window exists is reapplied when the form is shown and after
	   its content-fit resize.
	2. Forms now show an icon in the header bar only when Icon is explicitly set; changing Icon
	   after the window is created updates the title bar immediately, and setting it to null clears
	   the image.
	3. Wired Form.Activated and Form.Deactivate to the native window active state, with duplicate
	   notifications suppressed. Deferred modal closes now destroy the native window after the
	   modal loop completes, so dialogs no longer remain visible after Close.
	4. Replaced Net4x.NuGetUtility with Net4x.CleanCache and added reliable build-time eviction of
	   the package's own NuGet cache entry, together with a refresh script for build-server cache
	   locks.
	5. Added the opt-in Gtk.Windows.Forms.InternetExplorer companion package, hosting the legacy
	   Windows Trident WebBrowser control through OLE/ActiveX. It supports navigation, state and
	   browser events, DOM access and script invocation, print/document dialogs, IE11 emulation,
	   dedicated STA hosting for MTA applications, and configurable priority over WebView2. Added
	   registration, probe, command-queue, bounds, emulation, and STA-apartment tests.

 #### 2026/11/7
   	1. Fixed anchoring: each child now remembers the container size its bounds correspond to (WinForms
	   keeps per-control distances), instead of one baseline per container. A window enlarged after
	   construction but before its first real allocation (in Load, before Show, or in Shown) now moves
	   its anchored children, and a child that was invisible during a resize is correct when it is
	   shown. The titlebar / content-fit compensation still keeps anchored children at their designed
	   positions (patch 0159).
	2. Menu and tool-strip captions treat '&' as a mnemonic, as WinForms does: "&File" shows File with
	   the F underlined, "&&" shows one '&', a trailing '&' is dropped, and '_' stays a literal
	   underscore. Applies to ToolStripMenuItem (menu bar, submenus, context menus), ToolStripButton,
	   ToolStripLabel, ToolStripStatusLabel, ToolStripDropDownButton and ToolStripSplitButton; Text still
	   reads back as written. The underline is always drawn (as for Button/Label), and Alt+key activates
	   the item through GTK's mnemonic mechanism (patch 0160).
	3. The access key of every caption (Button, CheckBox, RadioButton, Label, GroupBox and the
	   menu / tool-strip items above) is now underlined at rest: GTK 4 only draws its own mnemonic
	   underline while Alt is held, so the key is also marked with explicit underline markup (no
	   double underline when Alt is held; Alt+key still activates). Fixed ToolStripStatusLabel
	   constructors ignoring their text/image/onClick/name arguments, and tray-menu labels turning a
	   trailing '&' into a mnemonic instead of dropping it (patch 0162).
	3. Anchoring follow-up: invisible children are anchored too (their bounds are current while hidden, including after a
	   maximize), a control added after layout started follows the next resize, the first-allocation delta of a form is
	   measured from its designed client size against the size the window really opened at (a Width shrink GTK does not
	   honour, a docked status strip and a form designed maximized no longer shift anchored children), Control.ClientSize
	   re-bases the anchor, and visibility changes re-run layout only when the visibility changed (deferred while layout
	   is suspended) (patch 0161).
	4. Anchoring, second follow-up: a StatusStrip added first no longer corrupts new anchors or DisplayRectangle; a control
	   removed and re-added or moved to another parent is positioned for its new parent; a hidden AutoSize control is
	   moved but no longer collapsed to 0x0 (no size request is stamped on an axis the control does not constrain);
	   ClientSize assigned after the designer, and Height/Width changed on a form designed with Size only, now move the
	   children; scaling a form (Scale, auto-scale) is no longer counted as a resize; a control added to a runtime
	   container is positioned for it unless its layout is suspended; a child pushed past an edge is restored when the
	   container grows back (patch 0163).
	5. SplitContainer: SplitterDistance now persists while the control is hidden or has no size (GTK clamps its own
	   position then and never restores it) and is applied once the control is allocated and again after it is shown;
	   added the SplitterMoving (cancellable) and SplitterMoved events, raised for user drags and for a changed
	   SplitterDistance but not for GTK's clamping or a resize; FixedPanel.None now lets both panels resize
	   (patch 0164). Control.ClientSize no longer reports the 2x2 allocation a widget has just after it is
	   shown (before its first layout) but its requested size, which made code sizing against a freshly shown
	   panel compute nonsense (patch 0165).

 #### 2026/9/26
   	1. Fixed the Windows Forms designer for projects that reference Gtk.Windows.Forms.Base: the
	   Gtk.Windows.FormsDesigner targets never reached them, so the designer runtimeconfig kept the
	   SDK's Microsoft.NETCore.App-only frameworks, the out-of-process designer server could not load
	   System.Windows.Forms, and Visual Studio reported a ServerProcess failure. The package now ships
	   its props/targets in buildTransitive/ as well, and Gtk.Windows.Forms.Base lets its Build assets
	   flow (its runtime assets no longer do, so Microsoft.Build.* stays out of consumers' output).
	2. The designer configuration is rewritten after Visual Studio's designer output group too, not
	   only before a build: a restore between two builds used to be enough to break the designer.
	3. The project file is rewritten only when a rule actually changes it, and never from the
	   design-time pass.

 #### 2026/8/16
   	1. Added the WebBrowser control and the Html* DOM layer (WebBrowser, WebBrowserBase,
	   WebBrowserSiteBase, HtmlDocument, HtmlElement, HtmlWindow, HtmlHistory, the two collections,
	   their event args and handlers, and three enums) - ported from the mono submodule with the
	   Mozilla/XPCOM DOM replaced by a JavaScript bridge.
	2. Engines: WebKitGTK 6.0 on Linux and macOS, in the library; Microsoft Edge WebView2 on
	   Windows, in the new companion package Gtk.Windows.Forms.WebView2. Where neither is installed
	   the control still constructs and still raises its navigation events.
	3. 【!】Both engines run the page out of process, so Html* getters read a cached snapshot and
	   InvokeScript/InvokeMember post the call and return null. Gtk.Windows.Extensions carries the
	   asynchronous siblings that return a value. See EMULATION-GAPS.md 2.8.
	4. Fixed Library.Webkit, which named the GTK 3 webkit2gtk-4.0 binding and could only ever fail
	   to load on this branch (patch 0147).

 #### 2026/7/10
   	1. Updated Control base functionality, PropertyGrid, GroupBox, Label
 	2. Reworked and optimized the control base code; added MDIForm control support.
 #### 2026/3/11
  	1. Removed the automatic word-wrap of Button text;
	2. Updated FlowLayoutPanel/TableLayoutPanel;
  	3. Optimized and fixed some base events and properties;
	4. Fixed PictureBox events and properties;
	5. 【!】Major change: refactored the container structure and the Dock/Anchor layout code
 #### 2026/2/27
  	1. Updated and fixed Control base properties and methods;
	2. Reworked the TableLayoutPanel, FlowLayoutPanel and GroupBox controls;
	3. Implemented Tab-key navigation
 #### 2026/2/5
  	1. Fixed the cursor property; switched to a built-in cursor
	2. Updated the container layout code
	3. Fixed ToolTip functionality
	4. Fixed touch-screen events
 #### 2026/1/11
  	1. 【!】Optimized CheckedListBox (older versions could become sluggish)
	2. Optimized ListView; added column-width dragging
	3. 【!】Added AOT compilation support — note there are extensive code changes
 #### 2025/12/24
 	1. 【!】Reworked DataGridView core functionality and properties
	2. Updated ComboBox
	3. Added a Screen class implementation
	4. Completed the Graphics string-drawing methods
	5. 【!】Optimized the container layout code — more accurate dynamic Dock/Anchor layout
	6. Fixed some base functionality
 #### 2025/12/13
 	1. Fixed DataGridView value-row indexing, events and styles
	2. Updated the base event code
 #### 2025/11/27
 	1. Updated menu background color and menu layout
	2. Updated control keyboard/mouse events
	3. Updated window title-bar button functionality
	4. Optimized the ComboBox control
	5. Optimized the ListView control
	6. Updated DataGridView; optimized multi-level style properties
 #### 2025/11/1
	1. Updated menu styling
	2. Updated window functionality
 #### 2025/10/25
	1. Updated row add/remove functionality for DataGridView and TreeView
 #### 2025/10/22
	1. Fixed image, style and resource-path issues; supports launching the app from any location
	2. Optimized some code
 #### 2025/10/17
	1. Implemented hiding the window's taskbar icon on Windows
	2. Implemented free control over showing the window maximize/minimize buttons
	3. Optimized some code
 #### 2025/9/14
	1. Updated menus
	2. Updated container functionality
	3. [!] Updated the project's form-designer usage approach — now compatible with using native projects at the same time; strongly recommended (already updated in the demo projects)
 #### 2025/8/15
	1. Optimized ListView
	2. Fixed events and properties of some controls
	3. Fixed and optimized container properties
	4. Updated the structure of some controls
	5. Added a UI-styling development sample
 #### 2025/8/15
	1. [!] Fixed an issue where the window title bar could be missing the resize buttons
 #### 2025/8/14
	1. [!] Refactored the menu bar and improved its functionality
	2. Updated window-state functionality
	3. Optimized the control style-configuration functionality
	4. [!] Resolved the image resource-directory issue
	5. Added and optimized control properties and functionality
 #### 2025/7/24
	1. [!] Updated and optimized window events — significant improvements to window events and their hierarchy
 #### 2025/7/24
	1. Added the NotifyIcon component
	2. Updated the image-processing code
 #### 2025/7/6
	1. Added EndEdit support to DataGridView
	2. Updated DataGridView column-handling functionality
 #### 2025/6/29
	1. Optimized mouse events
 #### 2025/6/13
 	1. Updated control completion events to be unique
 #### 2025/5/19
	1. Optimized size control of container controls
	2. Added class libraries; fixed discovered issues
	3. Updated TableLayoutPanel
	4. Updated PictureBox with animated-image support
 #### 2025/4/29
	1. Updated the PictureBox and ListView controls
	2. Updated the ListBox and RichTextBox controls; added content scrolling
 #### 2025/4/20
	1. Changed containers so they are no longer stretched by inner child controls
	2. Updated some control event methods and properties
	3. Updated the font-configuration functionality
	4. Optimized Graphics
 #### 2025/3/10
	1. Fixed mouse events
	2. Added Control event methods
 #### 2025/3/10
	1. Fixed a file-picker memory-conflict issue and the filter format
	2. Updated dialog-window icons
	3. Fixed child-window closing issues and the container scrollbar-not-showing issue
	4. Updated ComboBox data binding and properties
	5. Changed the window image resource-directory structure; added a form-designer usage demo project
	6. Fixed the Graphics font-configuration code
	7. Updated ListView
 #### 2025/2/27
	1. Fixed the DataGridView sorting-disabled property
	2. Fixed the FileDialog FileName property
 #### 2025/2/26
	1. Updated TextBox\CheckBox\ListView\DataGridView
	2. Fixed and optimized some functionality
	3. Added the SwitchBox development control
	4. Added the PropertyGrid control
 #### 2025/2/9
	1. Updated ListView\TabControl, optimized functionality, fixed a ListView scrollbar bug
	2. Fixed container layout precision and mechanism
	3. Added netstandard2.0 support
 
 #### 2025/2/5
	1. Updated the default display type of ToolStripButton
	2. Updated ToolStrip to support right-aligning the last menu item
	3. Major ListView update — added implementation methods and improved the UI
	4. Updated Form and fixed a bug
 #### 2025/1/2
	1. Updated the layout functionality of container controls.
	2. Updated DataGridView data-binding functionality.
	3. Updated the window close functionality.
 #### 2024/12/20
	1. Updated listview, listbox, checkedlistbox and datagridview functionality.
 #### 2024/12/14
	1. Implemented the control SizeChange event.
	2. Updated message-box content wrapping and datagridview auto-wrapping
	3. Updated listview add/remove functionality
	4. Fixed some discovered issues
 #### 2024/11/7
	1. Fixed container-control functionality.
	2. Completed treeview functionality
	3. Implemented basic ImageList component configuration and image retrieval
	4. Fixed some discovered issues
 #### 2024/10/23
	1. [Major update] Completed container-control functionality and fixed errors.
	2. Completed Graphics/Image drawing functionality
	3. Fixed some discovered issues
 #### 2024/10/19
	1. [Major update] Refactored the container layout, improving dock/Anchor with better performance and more stable multi-level nesting.
	2. Implemented datagridview auto-wrapping functionality and properties
	3. Added operations to bring a Form to the top, activate it, and get already-open forms
	4. Fixed some discovered issues
 #### 2024/9/28
   1. Fixed the window Paint event and the CreateGraphics method
   2. Added the ScrollBar and FontDialog controls
   3. Fixed the SelectIndexChanged\SelectValueChanged\SelectItemChanged events
 #### 2024/8/28
   1. Implemented right-click menus for all controls
   2. Added theme-configuration and style-configuration functionality
 #### 2024/8/28
   1. Adjusted and optimized control position/border properties for precise alignment.
   2. Added some missing control events
 #### 2024/8/27
   1. Reworked and optimized window-closing behavior; unified the Windows and Linux window title bars, improving compatibility when reopening a native window after closing.
   2. Implemented DataGridView cell-style (CellStyle) functionality; reworked the cell image-loading code, optimized performance and fixed bugs
   3. Optimized the base dataset library
   4. Optimized the printing component; added a print-preview control and print-preview window
 #### 2024/7/16
   1. Reworked and optimized the control event code
 #### 2024/7/16
   1. Fixed the open-file dialog and message pop-ups
 #### 2024/7/12
   1. Added a printing component
   2. Fixed some issues
 #### 2024/7/7
   1. Changed some control styles
 #### 2024/6/28
   1. Changed some border styles and control mouse events
   2. Fixed some control property values / errors
   3. Refactored the container scrolling-window architecture
 #### 2024/6/22
   1. Fixed some bugs
   2. Reworked and optimized the resize functionality of windows and controls
 #### 2024/6/20
   1. Fixed some bugs
   2. Reworked and optimized the resize functionality of windows and controls
   3. Added mouse events to UserControl
 #### 2024/6/19
   1. Style adjustments
   2. Fixed some bugs
   3. Optimized the resize functionality of windows and controls
   4. Implemented adjustable control position and size properties
   5. Implemented mouse-cursor style properties for some controls
 
 #### 2024/6/10
   1. Changed how control background images are displayed, so most controls support rounded corners and rounded background-image display (important)
   2. Reworked and optimized the functionality and performance of many controls
   3. Reworked and optimized the control style-display code to support a theme/skinning mechanism
   4. Fixed some discovered functional or code errors
   5. Added asynchronous URL-image loading to DataGridView; optimized DataGridView data-display performance
 #### 2024/5/30
   1. (Important) Fixed the invoker synchronization method for multi-threaded UI updates; Timer execution is now synchronized with the UI
   2. Fixed the background color of ListBox, ListView and RichTextBox
   3. Enhanced the project feature-demo samples; added a demo of changing/scrolling data
 #### 2024/5/28
   1. Fixed and added some control methods
   2. Added a GTK multi-threaded UI-update mechanism
 #### 2024/5/21
   1. combobox\listbox functionality
   2. Fixed ToolStripSeparator
 #### 2024/5/17
   1. Added time data and a format mode to DateTimePicker
 #### 2024/5/16
   1. Fixed an issue where the Form window had a transparent border line
   2. Added several commonly used control properties
   3. Added selectable DropDown / DropDownList modes to the ComboBox control
   4. Fixed functional errors in the Visual Studio development plugin; improved installation adaptability and compatibility
 #### 2024/5/11
   1. Fixed abnormal window size when a Form starts
   2. Added an Image property/image to Button
   3. Fixed control background positioning
 #### 2024/5/6
   1. Completed treeview and listview functionality
   2. Added the GTKSystem.Windows.FormsDesigner.dll development tool (installed via NuGet), which automatically checks and fixes the form-designer configuration at build time
   3. Fixed the data-retrieval error in datagridview
 #### 2024/5/1
  1. Major update! Refactored the control structure code, optimized the functionality and performance of many controls, and fixed some errors
  2. Optimized the drawing and control-background code; drawing a background image no longer covers child controls
  3. Specially optimized the Form UI code and performance
 
 #### 2024/4/20
 1. Fixed the drawing position of graphics
 2. Implemented graphics-path drawing and gradient colors
 3. Implemented the BeginInvoke and EndInvoke methods on controls
 4. Reworked the data-loading code of DataGridView and ListBox; fixed the issue where data could not be loaded during window startup
 #### 2024/3/27
 1. Fixed the issue where UserControl threw an exception when opened in the form designer (controls still could not be displayed)
 2. Implemented ellipse drawing on Graphics
 #### 2024/3/19
 1. Panel shows scrollbars when content overflows; optimized the window-scaling code
 #### 2024/3/14
 1. Fixed TreeView data loading
 #### 2024/3/6
 1. Fixed some window-configuration issues and binding
 #### 2024/3/2
 1. Fixed the font-size issue of Label text; added alignment properties
 2. Implemented ImageList compatibility with the form designer
 #### 2024/2/29
 1. Added curve and polygon drawing to Graphics; optimized the text-drawing code
 2. Fixed some latent exceptions
 #### 2024/2/23
 1. Implemented and fixed cell-control data editing and retrieval in DataGridView
