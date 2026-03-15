// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal partial class PropertyStore
{
    private sealed class PaddingWrapper
    {
        public Padding Padding;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.PropertyStore.PaddingWrapper" /> class.
        /// </summary>
        /// <param name="padding">The padding.
        /// </param>
        public PaddingWrapper(Padding padding)
        {
            Padding = padding;
        }

    }
}