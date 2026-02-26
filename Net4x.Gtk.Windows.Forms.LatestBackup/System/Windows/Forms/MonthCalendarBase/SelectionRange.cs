// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// This is a class that represents the date selection range of a MonthCalendar control.
    /// </summary>
    [
    TypeConverter(typeof(SelectionRangeConverter))
    ]
    public sealed class SelectionRange
    {
        /// <summary>
        /// The lower limit of the selection range.
        /// </summary>
        private DateTime start = DateTime.MinValue.Date;

        /// <summary>
        /// The upper limit of the selection range.
        /// </summary>
        private DateTime end = DateTime.MaxValue.Date;

        /// <summary>
        /// Create a new SelectionRange object with the range [null, null].
        /// </summary>
        public SelectionRange()
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.SelectionRange" /> class with the specified beginning and ending dates.
        /// </summary>
        /// <param name="lower">The starting date in the
        /// <see cref="System.Windows.Forms.SelectionRange" />
        /// </param>
        /// <param name="upper">The ending date in the
        /// <see cref="System.Windows.Forms.SelectionRange" />
        /// </param>
        public SelectionRange(DateTime lower, DateTime upper)
        {
            //NOTE: simcooke: we explicitly DO NOT want to throw an exception here - just silently
            //                swap them around. This is because the win32 control can return non-
            //                normalized ranges.

            // We use lower.Date and upper.Date to remove any time component
            //
            if (lower < upper)
            {
                start = lower.Date;
                end = upper.Date;
            }
            else
            {
                start = upper.Date;
                end = lower.Date;
            }
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.SelectionRange" /> class with the specified selection range.
        /// </summary>
        /// <param name="range">The existing
        /// <see cref="System.Windows.Forms.SelectionRange" />
        /// </param>
        public SelectionRange(SelectionRange range)
        {
            start = range.start;
            end = range.end;
        }

        /// <summary>
        /// Gets or sets the ending date and time of the selection range.
        /// </summary>
        /// <returns>The ending
        /// <see cref="System.DateTime" /> value of the range.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DateTime End
        {
            get
            {
                return end;
            }
            set
            {
                end = value.Date;
            }
        }

        /// <summary>
        /// Gets or sets the starting date and time of the selection range.
        /// </summary>
        /// <returns>The starting
        /// <see cref="System.DateTime" /> value of the range.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public DateTime Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value.Date;
            }
        }

        /// <summary>
        /// Returns a string that represents the
        /// <see cref="System.Windows.Forms.SelectionRange" />
        /// </summary>
        /// <returns>A string that represents the current
        /// <see cref="System.Windows.Forms.SelectionRange" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string ToString()
        {
            return "SelectionRange: Start: " + start.ToString() + ", End: " + end.ToString();
        }
    }
}
