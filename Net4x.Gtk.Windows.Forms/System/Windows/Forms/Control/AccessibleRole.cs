namespace System.Windows.Forms;

/// <summary>
/// Specifies values representing possible roles for an accessible object.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
/// <remarks>
/// This enum declared only <see cref="F:System.Windows.Forms.AccessibleRole.Default" />, so any source
/// naming a real role failed to compile. The members and their values match
/// <c>System.Windows.Forms.AccessibleRole</c>, which in turn mirrors the MSAA <c>ROLE_SYSTEM_*</c>
/// constants.
/// </remarks>
public enum AccessibleRole
{
    /// <summary>A system-provided role.</summary>
    Default = -1,

    /// <summary>No role.</summary>
    None = 0,

    /// <summary>A title or caption bar for a window.</summary>
    TitleBar = 1,

    /// <summary>A menu bar.</summary>
    MenuBar = 2,

    /// <summary>A scroll bar.</summary>
    ScrollBar = 3,

    /// <summary>A special mouse pointer that resizes a window pane or split control.</summary>
    Grip = 4,

    /// <summary>A system sound.</summary>
    Sound = 5,

    /// <summary>The mouse pointer.</summary>
    Cursor = 6,

    /// <summary>The caret.</summary>
    Caret = 7,

    /// <summary>An alert or condition to notify the user about.</summary>
    Alert = 8,

    /// <summary>A window frame.</summary>
    Window = 9,

    /// <summary>A window's client area.</summary>
    Client = 10,

    /// <summary>A pop-up menu.</summary>
    MenuPopup = 11,

    /// <summary>A menu item.</summary>
    MenuItem = 12,

    /// <summary>A tool tip.</summary>
    ToolTip = 13,

    /// <summary>The main window of an application.</summary>
    Application = 14,

    /// <summary>A document window.</summary>
    Document = 15,

    /// <summary>A separate area of a frame or document window.</summary>
    Pane = 16,

    /// <summary>A graphical chart.</summary>
    Chart = 17,

    /// <summary>A dialog box or message box.</summary>
    Dialog = 18,

    /// <summary>A window border.</summary>
    Border = 19,

    /// <summary>Logically grouped objects.</summary>
    Grouping = 20,

    /// <summary>A visual separator between two areas.</summary>
    Separator = 21,

    /// <summary>A toolbar.</summary>
    ToolBar = 22,

    /// <summary>A status bar.</summary>
    StatusBar = 23,

    /// <summary>A table of rows and columns.</summary>
    Table = 24,

    /// <summary>A column header.</summary>
    ColumnHeader = 25,

    /// <summary>A row header.</summary>
    RowHeader = 26,

    /// <summary>A column of cells.</summary>
    Column = 27,

    /// <summary>A row of cells.</summary>
    Row = 28,

    /// <summary>A cell within a table.</summary>
    Cell = 29,

    /// <summary>A link.</summary>
    Link = 30,

    /// <summary>A help balloon.</summary>
    HelpBalloon = 31,

    /// <summary>A mouse pointer shaped like a character.</summary>
    Character = 32,

    /// <summary>A list box.</summary>
    List = 33,

    /// <summary>An item in a list box.</summary>
    ListItem = 34,

    /// <summary>An outline or tree structure.</summary>
    Outline = 35,

    /// <summary>An item in an outline or tree structure.</summary>
    OutlineItem = 36,

    /// <summary>A property page tab.</summary>
    PageTab = 37,

    /// <summary>A property page.</summary>
    PropertyPage = 38,

    /// <summary>An indicator such as a pointer graphic.</summary>
    Indicator = 39,

    /// <summary>A picture.</summary>
    Graphic = 40,

    /// <summary>Read-only text.</summary>
    StaticText = 41,

    /// <summary>Selectable or editable text.</summary>
    Text = 42,

    /// <summary>A push button.</summary>
    PushButton = 43,

    /// <summary>A check box.</summary>
    CheckButton = 44,

    /// <summary>An option or radio button.</summary>
    RadioButton = 45,

    /// <summary>A combo box.</summary>
    ComboBox = 46,

    /// <summary>A drop-down list box.</summary>
    DropList = 47,

    /// <summary>A progress bar.</summary>
    ProgressBar = 48,

    /// <summary>A dial or knob.</summary>
    Dial = 49,

    /// <summary>A hot-key field for a shortcut.</summary>
    HotkeyField = 50,

    /// <summary>A slider.</summary>
    Slider = 51,

    /// <summary>A spin box.</summary>
    SpinButton = 52,

    /// <summary>A diagram.</summary>
    Diagram = 53,

    /// <summary>An animation control.</summary>
    Animation = 54,

    /// <summary>A mathematical equation.</summary>
    Equation = 55,

    /// <summary>A button that drops down a list of items.</summary>
    ButtonDropDown = 56,

    /// <summary>A button that drops down a menu.</summary>
    ButtonMenu = 57,

    /// <summary>A button that drops down a grid.</summary>
    ButtonDropDownGrid = 58,

    /// <summary>A blank space between other objects.</summary>
    WhiteSpace = 59,

    /// <summary>A container of page tabs.</summary>
    PageTabList = 60,

    /// <summary>A control that displays the time.</summary>
    Clock = 61,

    /// <summary>A toolbar button with a drop-down list.</summary>
    SplitButton = 62,

    /// <summary>An IP address control.</summary>
    IpAddress = 63,

    /// <summary>An outline button that expands or collapses.</summary>
    OutlineButton = 64
}