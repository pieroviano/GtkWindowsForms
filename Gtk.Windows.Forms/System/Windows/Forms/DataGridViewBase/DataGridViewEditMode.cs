// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Defines values for specifying how a cell begins edit mode.
/// </summary>
public enum DataGridViewEditMode
{
    /// <summary>Editing begins when the cell receives focus.</summary>
    EditOnEnter = 0,
    /// <summary>Editing begins when a text-entry key is pressed.</summary>
    EditOnKeystroke = 1,
    /// <summary>Editing begins when a text-entry key or F2 is pressed.</summary>
    EditOnKeystrokeOrF2 = 2,
    /// <summary>Editing begins when F2 is pressed with the cell selected.</summary>
    EditOnF2 = 3,
    /// <summary>Editing begins only in response to BeginEdit.</summary>
    EditProgrammatically = 4
}
