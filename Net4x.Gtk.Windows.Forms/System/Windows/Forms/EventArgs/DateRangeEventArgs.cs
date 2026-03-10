namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.MonthCalendar.DateChanged" /> or
/// <see cref="T:System.Windows.Forms.MonthCalendar.DateSelected" /> events of the
/// <see cref="T:System.Windows.Forms.MonthCalendar" /> control.
/// </summary>
public class DateRangeEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateRangeEventArgs"/> class.
    /// </summary>
    /// <param name="start">The start.</param>
    /// <param name="end">The end.</param>
    public DateRangeEventArgs(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Gets the end.
    /// </summary>
    /// <value>The end.</value>
    public DateTime End { get; }

    /// <summary>
    /// Gets the start.
    /// </summary>
    /// <value>The start.</value>
    public DateTime Start { get; }

}
