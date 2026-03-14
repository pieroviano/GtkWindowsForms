// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
/// Stores
/// <see cref="T:System.Windows.Forms.InputLanguage" /> objects.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class InputLanguageCollection : ReadOnlyCollectionBase
{
    /// <summary>
    /// Initializes a new instance of
    /// <see cref="T:System.Windows.Forms.InputLanguageCollection" /> containing any array of
    /// <see cref="T:System.Windows.Forms.InputLanguage" /> objects.
    /// </summary>
    internal InputLanguageCollection(InputLanguage[] value)
    {
        InnerList.AddRange(value);
    }

    /// <summary>
    /// Represents the entry at the specified index of the
    /// <see cref="T:System.Windows.Forms.InputLanguage" />.
    /// </summary>
    public InputLanguage this[int index]
    {
        get
        {
            return ((InputLanguage)(InnerList[index]));
        }
    }

    /// <summary>
    /// Gets a value indicating whether the
    /// <see cref="T:System.Windows.Forms.InputLanguageCollection" /> contains the specified
    /// <see cref="T:System.Windows.Forms.InputLanguage" />.
    /// </summary>
    public bool Contains(InputLanguage value)
    {
        return InnerList.Contains(value);
    }

    /// <summary>
    /// Copies the
    /// <see cref="T:System.Windows.Forms.InputLanguageCollection" /> values to a one-dimensional
    /// <see cref="T:System.Array" /> instance at the specified index.
    /// </summary>
    public void CopyTo(InputLanguage[] array, int index)
    {
        InnerList.CopyTo(array, index);
    }

    /// <summary>
    /// Returns the index of a
    /// <see cref="T:System.Windows.Forms.InputLanguage" /> in the
    /// <see cref="T:System.Windows.Forms.InputLanguageCollection" /> .
    /// </summary>
    public int IndexOf(InputLanguage value)
    {
        return InnerList.IndexOf(value);
    }

}