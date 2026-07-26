// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies SecurityIDType.</summary>
public enum SecurityIDType
{
    /// <summary>The User value.</summary>
    User = 1,

    /// <summary>The Group value.</summary>
    Group = 2,

    /// <summary>The Domain value.</summary>
    Domain = 3,

    /// <summary>The Alias value.</summary>
    Alias = 4,

    /// <summary>The WellKnownGroup value.</summary>
    WellKnownGroup = 5,

    /// <summary>The DeletedAccount value.</summary>
    DeletedAccount = 6,

    /// <summary>The Invalid value.</summary>
    Invalid = 7,

    /// <summary>The Unknown value.</summary>
    Unknown = 8,

    /// <summary>The Computer value.</summary>
    Computer = 9,
}
