// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Represents an entry in a file dialog's custom places collection, identified by a path or a known-folder GUID.</summary>
public class FileDialogCustomPlace
{
    /// <summary>Initializes a new instance from a file-system path.</summary>
    /// <param name="path">The path of the custom place.</param>
    public FileDialogCustomPlace(string path)
    {
        Path = path ?? string.Empty;
        KnownFolderGuid = Guid.Empty;
    }

    /// <summary>Initializes a new instance from a known-folder GUID.</summary>
    /// <param name="knownFolderGuid">The known-folder GUID of the custom place.</param>
    public FileDialogCustomPlace(Guid knownFolderGuid)
    {
        Path = string.Empty;
        KnownFolderGuid = knownFolderGuid;
    }

    /// <summary>Gets or sets the file-system path of the custom place.</summary>
    public string Path { get; set; }

    /// <summary>Gets or sets the known-folder GUID of the custom place.</summary>
    public Guid KnownFolderGuid { get; set; }

    /// <summary>Returns a string that represents this custom place.</summary>
    /// <returns>A string describing the path and GUID.</returns>
    public override string ToString() => "Path: " + Path + ", KnownFolderGuid: " + KnownFolderGuid;
}
