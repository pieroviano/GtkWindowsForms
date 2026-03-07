// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.ScrollBar.Scroll" /> event.
    /// </summary>
    public class ScrollEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ScrollEventArgs" /> class using the given values for the
        /// <see cref="System.Windows.Forms.ScrollEventArgs.Type" /> and
        /// <see cref="System.Windows.Forms.ScrollEventArgs.NewValue" /> properties.
        /// </summary>
        /// <param name="type">One of the
        /// <see cref="System.Windows.Forms.ScrollEventType" /> values.
        /// </param>
        /// <param name="newValue">The new value for the scroll bar.
        /// </param>
        public ScrollEventArgs(ScrollEventType type, int newValue)
        {
            Type = type;
            NewValue = newValue;
            OldValue = -1;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ScrollEventArgs" /> class using the given values for the
        /// <see cref="System.Windows.Forms.ScrollEventArgs.Type" />,
        /// <see cref="System.Windows.Forms.ScrollEventArgs.NewValue" />, and
        /// <see cref="System.Windows.Forms.ScrollEventArgs.ScrollOrientation" /> properties.
        /// </summary>
        /// <param name="type">One of the
        /// <see cref="System.Windows.Forms.ScrollEventType" /> values.
        /// </param>
        /// <param name="newValue">The new value for the scroll bar.
        /// </param>
        /// <param name="scroll">One of the
        /// <see cref="System.Windows.Forms.ScrollOrientation" /> values.
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
        /// <see cref="System.Windows.Forms.ScrollEventArgs" /> class using the given values for the
        /// <see cref="System.Windows.Forms.ScrollEventArgs.Type" />,
        /// <see cref="System.Windows.Forms.ScrollEventArgs.OldValue" />, and
        /// <see cref="System.Windows.Forms.ScrollEventArgs.NewValue" /> properties.
        /// </summary>
        /// <param name="type">One of the
        /// <see cref="System.Windows.Forms.ScrollEventType" /> values.
        /// </param>
        /// <param name="oldValue">The old value for the scroll bar.
        /// </param>
        /// <param name="newValue">The new value for the scroll bar.
        /// </param>
        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
        {
            Type = type;
            NewValue = newValue;
            OldValue = oldValue;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ScrollEventArgs" /> class using the given values for the
        /// <see cref="System.Windows.Forms.ScrollEventArgs.Type" />,
        /// <see cref="System.Windows.Forms.ScrollEventArgs.OldValue" />,
        /// <see cref="System.Windows.Forms.ScrollEventArgs.NewValue" />, and
        /// <see cref="System.Windows.Forms.ScrollEventArgs.ScrollOrientation" /> properties.
        /// </summary>
        /// <param name="type">One of the
        /// <see cref="System.Windows.Forms.ScrollEventType" /> values.
        /// </param>
        /// <param name="oldValue">The old value for the scroll bar.
        /// </param>
        /// <param name="newValue">The new value for the scroll bar.
        /// </param>
        /// <param name="scroll">One of the
        /// <see cref="System.Windows.Forms.ScrollOrientation" /> values.
        /// </param>
        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll)
        {
            Type = type;
            NewValue = newValue;
            OldValue = oldValue;
            ScrollOrientation = scroll;
        }

        /// <summary>
        /// Gets or sets the new
        /// <see cref="System.Windows.Forms.ScrollBar.Value" /> of the scroll bar.
        /// </summary>
        /// <returns>The numeric value that the
        /// <see cref="System.Windows.Forms.ScrollBar.Value" /> property will be changed to.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int NewValue { get; set; }

        /// <summary>
        /// Gets the old
        /// <see cref="System.Windows.Forms.ScrollBar.Value" /> of the scroll bar.
        /// </summary>
        /// <returns>The numeric value that the
        /// <see cref="System.Windows.Forms.ScrollBar.Value" /> property contained before it changed.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int OldValue { get; }

        /// <summary>
        /// Gets the scroll bar orientation that raised the Scroll event.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ScrollOrientation" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ScrollOrientation ScrollOrientation { get; }

        /// <summary>
        /// Gets the type of scroll event that occurred.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ScrollEventType" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public ScrollEventType Type { get; }

    }
}
