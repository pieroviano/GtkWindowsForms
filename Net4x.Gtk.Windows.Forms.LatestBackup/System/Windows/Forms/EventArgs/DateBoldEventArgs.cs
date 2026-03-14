// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// The month calendar control fires this event to request information about how
/// the days within the visible months should be displayed.
/// </summary>
public class DateBoldEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DateBoldEventArgs" /> class.
    /// </summary>
    /// <param name="start">
    /// The start.
    /// </param>
    /// <param name="size">
    /// The size.
    /// </param>
    internal DateBoldEventArgs(DateTime start, int size)
    {
        StartDate = start;
        Size = size;
    }

    /// <summary>
    /// Gets or sets the days to bold.
    /// </summary>
    /// <value>
    /// The days to bold.
    /// </value>
    public int[] DaysToBold { get; set; }

    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>
    /// The size.
    /// </value>
    public int Size { get; }

    /// <summary>
    /// Gets the start date.
    /// </summary>
    /// <value>
    /// The start date.
    /// </value>
    public DateTime StartDate { get; }

}