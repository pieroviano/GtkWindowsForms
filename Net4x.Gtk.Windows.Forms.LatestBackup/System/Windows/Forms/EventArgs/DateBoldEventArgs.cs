// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// The month calendar control fires this event to request information about how
    /// the days within the visible months should be displayed.
    /// </summary>
    public class DateBoldEventArgs : EventArgs
    {
        internal DateBoldEventArgs(DateTime start, int size)
        {
            StartDate = start;
            Size = size;
        }

        /// <summary>
        /// Gets or sets dates that are bold.
        /// </summary>
        /// <returns>
        /// The dates that are bold.
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        public int[] DaysToBold { get; set; }

        /// <summary>
        /// Gets the number of dates that are bold.
        /// </summary>
        /// <returns>
        /// The number of dates that are bold.
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        public int Size { get; }

        /// <summary>
        /// Gets the first date that is bold.
        /// </summary>
        /// <returns>
        /// The first date that is bold.
        /// </returns>
        /// <filterpriority>
        /// 1
        /// </filterpriority>
        public DateTime StartDate { get; }

    }
}
