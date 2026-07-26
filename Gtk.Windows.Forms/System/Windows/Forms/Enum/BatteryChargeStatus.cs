// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Defines the battery charge status.</summary>
[Flags]
public enum BatteryChargeStatus
{
    /// <summary>The battery is at a high charge level.</summary>
    High = 1,
    /// <summary>The battery is at a low charge level.</summary>
    Low = 2,
    /// <summary>The battery is at a critically low charge level.</summary>
    Critical = 4,
    /// <summary>The battery is charging.</summary>
    Charging = 8,
    /// <summary>There is no system battery.</summary>
    NoSystemBattery = 128,
    /// <summary>The battery charge status is unknown.</summary>
    Unknown = 255,
}
