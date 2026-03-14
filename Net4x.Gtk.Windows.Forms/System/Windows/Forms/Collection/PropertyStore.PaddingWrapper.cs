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
        /// Initializes a new instance of the <see cref="PaddingWrapper"/> class.
        /// </summary>
        /// <param name="padding">The padding.</param>
        public PaddingWrapper(Padding padding)
        {
            Padding = padding;
        }

    }
}