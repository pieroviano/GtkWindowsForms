namespace System.Windows.Forms;

/// <summary>
/// Specifies the kind of action to take if a match is found when combining menu items on a
/// <see cref="T:System.Windows.Forms.ToolStrip" />
/// </summary>
public enum MergeAction
{
    /// <summary>
    /// The append
    /// </summary>
    Append,
    /// <summary>
    /// The insert
    /// </summary>
    Insert,
    /// <summary>
    /// The replace
    /// </summary>
    Replace,
    /// <summary>
    /// The remove
    /// </summary>
    Remove,
    /// <summary>
    /// The match only
    /// </summary>
    MatchOnly
}
