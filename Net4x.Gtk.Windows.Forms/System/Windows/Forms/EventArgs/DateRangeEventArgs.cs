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
        public DateRangeEventArgs(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
        }

        public DateTime End { get; }

        public DateTime Start { get; }
    }
}
