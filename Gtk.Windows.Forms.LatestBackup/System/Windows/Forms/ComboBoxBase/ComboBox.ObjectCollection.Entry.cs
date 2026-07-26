// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// ObjectCollection is the collection of items in the ComboBox. It is used when you want to add items to the ComboBox or retrieve items from it.
/// </summary>
/// <seealso cref="T:System.Collections.IList" />
public partial class ComboBox
{
    /// <summary>
    /// ObjectCollection is the collection of items in the ComboBox. It is used when you want to add items to the ComboBox or retrieve items from it.
    /// </summary>
    /// <seealso cref="T:System.Collections.IList" />
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

            public object Item
            {
                get;
                set;
            }

            public override string? ToString()
            {
                return Item.ToString();
            }
        }
    }
}