// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the system power status.</summary>
public enum PowerLineStatus
{
    /// <summary>The system is not connected to AC power.</summary>
    Offline = 0,
    /// <summary>The system is connected to AC power.</summary>
    Online = 1,
    /// <summary>The power status of the system is unknown.</summary>
    Unknown = 255,
}
