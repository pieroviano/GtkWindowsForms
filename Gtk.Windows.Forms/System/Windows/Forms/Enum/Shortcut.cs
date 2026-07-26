namespace System.Windows.Forms;

/// <summary>
/// Specifies shortcut keys that can be used by menu items.
/// </summary>
/// <remarks>
/// Part of the legacy menu API (<c>MainMenu</c>/<c>MenuItem</c>/<c>ContextMenu</c>), which was missing
/// entirely. Those types are obsolete in modern WinForms but are exactly what an application old enough to
/// need porting will be using. The values match the WinForms enumeration so that designer-generated code
/// and hand-written assignments compile unchanged.
/// </remarks>
public enum Shortcut
{
    /// <summary>No shortcut key.</summary>
    None = 0,

    /// <summary>The INSERT key.</summary>
    Ins = 45,

    /// <summary>The DELETE key.</summary>
    Del = 46,

    /// <summary>The F1 key.</summary>
    F1 = 112,

    /// <summary>The F2 key.</summary>
    F2 = 113,

    /// <summary>The F3 key.</summary>
    F3 = 114,

    /// <summary>The F4 key.</summary>
    F4 = 115,

    /// <summary>The F5 key.</summary>
    F5 = 116,

    /// <summary>The F6 key.</summary>
    F6 = 117,

    /// <summary>The F7 key.</summary>
    F7 = 118,

    /// <summary>The F8 key.</summary>
    F8 = 119,

    /// <summary>The F9 key.</summary>
    F9 = 120,

    /// <summary>The F10 key.</summary>
    F10 = 121,

    /// <summary>The F11 key.</summary>
    F11 = 122,

    /// <summary>The F12 key.</summary>
    F12 = 123,

    /// <summary>SHIFT+INSERT.</summary>
    ShiftIns = 65581,

    /// <summary>SHIFT+DELETE.</summary>
    ShiftDel = 65582,

    /// <summary>CTRL+INSERT.</summary>
    CtrlIns = 131117,

    /// <summary>CTRL+DELETE.</summary>
    CtrlDel = 131118,

    /// <summary>CTRL+A.</summary>
    CtrlA = 131137,

    /// <summary>CTRL+C.</summary>
    CtrlC = 131139,

    /// <summary>CTRL+N.</summary>
    CtrlN = 131150,

    /// <summary>CTRL+O.</summary>
    CtrlO = 131151,

    /// <summary>CTRL+P.</summary>
    CtrlP = 131152,

    /// <summary>CTRL+S.</summary>
    CtrlS = 131155,

    /// <summary>CTRL+V.</summary>
    CtrlV = 131158,

    /// <summary>CTRL+X.</summary>
    CtrlX = 131160,

    /// <summary>CTRL+Y.</summary>
    CtrlY = 131161,

    /// <summary>CTRL+Z.</summary>
    CtrlZ = 131162,

    /// <summary>CTRL+F4.</summary>
    CtrlF4 = 131187,

    /// <summary>ALT+F4.</summary>
    AltF4 = 262259
}
