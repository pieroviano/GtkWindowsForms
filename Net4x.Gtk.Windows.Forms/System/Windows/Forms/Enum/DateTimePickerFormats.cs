// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the date and time format the
    /// <see cref="System.Windows.Forms.DateTimePicker" />
    /// control displays.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum DateTimePickerFormat
    {
        /// <summary>
        /// The
        /// <see cref="System.Windows.Forms.DateTimePicker" /> control displays the date/time value in the long date format set by the user's operating system.
        /// </summary>
        Long = 0x0001,

        /// <summary>
        /// The
        /// <see cref="System.Windows.Forms.DateTimePicker" /> control displays the date/time value in the short date format set by the user's operating system.
        /// </summary>
        Short = 0x0002,

        /// <summary>
        /// The
        /// <see cref="System.Windows.Forms.DateTimePicker" /> control displays the date/time value in the time format set by the user's operating system.
        /// </summary>
        Time = 0x0004,

        /// <summary>
        /// The
        /// <see cref="System.Windows.Forms.DateTimePicker" /> control displays the date/time value in a custom format. For more information, see
        /// <see cref="System.Windows.Forms.DateTimePicker.CustomFormat" />
        /// </summary>
        Custom = 0x0008,
    }
}
