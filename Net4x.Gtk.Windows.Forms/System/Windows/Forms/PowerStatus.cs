// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Indicates the current system power status information. On the GTK layer, where no battery API is
/// wired, values default to "unknown / no battery".</summary>
public class PowerStatus
{
    internal PowerStatus() { }

    /// <summary>Gets the current system power status.</summary>
    public PowerLineStatus PowerLineStatus => PowerLineStatus.Unknown;

    /// <summary>Gets the current battery charge status.</summary>
    public BatteryChargeStatus BatteryChargeStatus => BatteryChargeStatus.NoSystemBattery;

    /// <summary>Gets the reported full charge lifetime of the battery, in seconds, or -1 if unknown.</summary>
    public int BatteryFullLifetime => -1;

    /// <summary>Gets the approximate percentage of full battery charge remaining (0.0 to 1.0), or 255 if unknown.</summary>
    public float BatteryLifePercent => 255f;

    /// <summary>Gets the approximate number of seconds of battery life remaining, or -1 if unknown.</summary>
    public int BatteryLifeRemaining => -1;
}
