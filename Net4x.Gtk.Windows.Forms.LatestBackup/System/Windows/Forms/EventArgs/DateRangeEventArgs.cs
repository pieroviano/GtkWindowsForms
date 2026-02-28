// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.MonthCalendar.DateChanged" /> or
    /// <see cref="System.Windows.Forms.MonthCalendar.DateSelected" /> events of the
    /// <see cref="System.Windows.Forms.MonthCalendar" /> control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class DateRangeEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.DateRangeEventArgs" /> class.
        /// </summary>
        /// <param name="start">The first date/time value in the range that the user has selected.
        /// </param>
        /// <param name="end">The last date/time value in the range that the user has selected.
        /// </param>
        public DateRangeEventArgs(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }
        /// <summary>
        /// Gets the first date/time value in the range that the user has selected.
        /// </summary>
        /// <returns>A
        /// <see cref="System.DateTime" /> that represents the first date in the date range that the user has selected.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DateTime Start { get; }
        /// <summary>
        /// Gets the last date/time value in the range that the user has selected.
        /// </summary>
        /// <returns>A
        /// <see cref="System.DateTime" /> that represents the last date in the date range that the user has selected.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DateTime End { get; }
    }
}
