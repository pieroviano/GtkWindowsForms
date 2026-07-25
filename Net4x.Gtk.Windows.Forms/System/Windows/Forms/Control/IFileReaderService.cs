// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;

namespace System.Windows.Forms;

/// <summary>Defines a service that opens a file stream from a named source.</summary>
public interface IFileReaderService
{
    /// <summary>Opens a stream over the file identified by <paramref name="fileName" />.</summary>
    /// <param name="fileName">The name of the file to open.</param>
    /// <returns>A readable <see cref="T:System.IO.Stream" />.</returns>
    Stream OpenFileFromSource(string fileName);
}
