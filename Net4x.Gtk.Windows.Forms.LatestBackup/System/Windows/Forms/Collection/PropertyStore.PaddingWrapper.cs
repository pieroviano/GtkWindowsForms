// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;
/// <summary>
/// Stores the relationship between a key and a value.
/// We do not want to be so inefficient that we require
/// four bytes for each four byte property, so use an algorithm
/// that uses the bottom two bits of the key to identify
/// one of four elements in an entry.
/// </summary>
internal partial class PropertyStore
{
    private sealed class PaddingWrapper
    {
        public Padding Padding;

        /// <summary>
        /// Initializes a new instance of the System.Windows.Forms.Padding class using the
        /// supplied padding size for all edges.
        /// </summary>
        /// <param name="all">
        /// The number of pixels to be used for padding for all edges.
        /// </param>
        public PaddingWrapper(Padding padding)
        {
            Padding = padding;
        }

    }
}