// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.ScrollBar.Scroll" /> event.
/// </summary>
public class ScrollEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ScrollEventArgs" /> class.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <param name="newValue">
    /// The new value.
    /// </param>
    public ScrollEventArgs(ScrollEventType type, int newValue)
    {
        Type = type;
        NewValue = newValue;
        OldValue = -1;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ScrollEventArgs" /> class.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <param name="newValue">
    /// The new value.
    /// </param>
    /// <param name="scroll">
    /// The scroll.
    /// </param>
    public ScrollEventArgs(ScrollEventType type, int newValue, ScrollOrientation scroll)
    {
        Type = type;
        NewValue = newValue;
        ScrollOrientation = scroll;
        OldValue = -1;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ScrollEventArgs" /> class.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <param name="oldValue">
    /// The old value.
    /// </param>
    /// <param name="newValue">
    /// The new value.
    /// </param>
    public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
    {
        Type = type;
        NewValue = newValue;
        OldValue = oldValue;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ScrollEventArgs" /> class.
    /// </summary>
    /// <param name="type">
    /// The type.
    /// </param>
    /// <param name="oldValue">
    /// The old value.
    /// </param>
    /// <param name="newValue">
    /// The new value.
    /// </param>
    /// <param name="scroll">
    /// The scroll.
    /// </param>
    public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll)
    {
        Type = type;
        NewValue = newValue;
        OldValue = oldValue;
        ScrollOrientation = scroll;
    }

    /// <summary>
    /// Specifies the new location of the scroll box within the scroll bar.
    /// </summary>
    /// <value>
    /// The new value.
    /// </value>
    public int NewValue
    {
        get;
        set;
    }

    /// <summary>
    /// Specifies the last position  within the scroll bar.
    /// </summary>
    /// <value>
    /// The old value.
    /// </value>
    public int OldValue { get; }

    /// <summary>
    /// Specifies the type of scroll event that occurred.
    /// </summary>
    /// <value>
    /// The scroll orientation.
    /// </value>
    public ScrollOrientation ScrollOrientation { get; }

    /// <summary>
    /// Specifies the type of scroll event that occurred.
    /// </summary>
    /// <value>
    /// The type.
    /// </value>
    public ScrollEventType Type { get; }

}