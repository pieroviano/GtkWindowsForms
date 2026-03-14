// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

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
    private sealed class SizeWrapper
    {
        public Size Size;

        public SizeWrapper(Size size)
        {
            Size = size;
        }

    }
}