// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;

namespace System.Windows.Forms;

/// <summary>Represents a collection of <see cref="T:System.Windows.Forms.FileDialogCustomPlace" /> objects.</summary>
public class FileDialogCustomPlacesCollection : Collection<FileDialogCustomPlace>
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.FileDialogCustomPlacesCollection" /> class.</summary>
    public FileDialogCustomPlacesCollection() { }

    /// <summary>Adds a custom place for the specified path to the collection.</summary>
    /// <param name="path">The path of the custom place.</param>
    public void Add(string path) => Add(new FileDialogCustomPlace(path));

    /// <summary>Adds a custom place for the specified known-folder GUID to the collection.</summary>
    /// <param name="knownFolderGuid">The known-folder GUID of the custom place.</param>
    public void Add(Guid knownFolderGuid) => Add(new FileDialogCustomPlace(knownFolderGuid));
}
