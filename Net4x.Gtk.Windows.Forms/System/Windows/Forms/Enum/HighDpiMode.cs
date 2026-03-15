using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
/// Enum HighDpiMode
/// </summary>
public enum HighDpiMode
{
    /// <summary>
    /// The dpi unaware
    /// </summary>
    DpiUnaware = 0,
    /// <summary>
    /// The system aware
    /// </summary>
    SystemAware = 1,
    /// <summary>
    /// The per monitor
    /// </summary>
    PerMonitor = 2,
    /// <summary>
    /// The per monitor v2
    /// </summary>
    PerMonitorV2 = 3,
    /// <summary>
    /// The dpi unaware GDI scaled
    /// </summary>
    DpiUnawareGdiScaled = 4
}