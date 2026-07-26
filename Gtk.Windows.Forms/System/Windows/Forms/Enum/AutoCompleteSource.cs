// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Specifies the source for automatic completion functionality.</summary>
public enum AutoCompleteSource
{
    /// <summary>Specifies the file system as the source.</summary>
    FileSystem = 1,

    /// <summary>Includes the Uniform Resource Locators (URLs) in the history list.</summary>
    HistoryList = 2,

    /// <summary>Includes the Uniform Resource Locators (URLs) in the list of those most recently used.</summary>
    RecentlyUsedList = 4,

    /// <summary>Specifies the equivalent of HistoryList and RecentlyUsedList as the source.</summary>
    AllUrl = 6,

    /// <summary>Specifies the equivalent of FileSystem and AllUrl as the source.</summary>
    AllSystemSources = 7,

    /// <summary>Specifies the file system directories as the source.</summary>
    FileSystemDirectories = 32,

    /// <summary>Specifies strings from a built-in AutoCompleteStringCollection as the source.</summary>
    CustomSource = 64,

    /// <summary>Automatic completion is disabled.</summary>
    None = 128,

    /// <summary>Specifies that the items of the ComboBox represent the source.</summary>
    ListItems = 256,
}
