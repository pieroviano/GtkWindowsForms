// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows combo box control.
    /// </summary>
    /// <filterpriority>1
    /// </filterpriority>
    public partial class ComboBox
    {
        public partial class ObjectCollection
        {
            /// <summary>
            /// This is a single entry in ObjectCollection.
            /// </summary>
            internal class Entry
            {
                public Entry(object item)
                {
                    Item = item;
                }

                public object Item { get; set; }

                public override string? ToString()
                {
                    return Item.ToString();
                }
            }
        }
    }
}
