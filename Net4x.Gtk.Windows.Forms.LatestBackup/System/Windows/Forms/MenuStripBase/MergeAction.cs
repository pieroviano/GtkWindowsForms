namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the kind of action to take if a match is found when combining menu items on a
    /// <see cref="System.Windows.Forms.ToolStrip" />
    /// </summary>
    /// <filterpriority> 2
    /// </filterpriority>
    public enum MergeAction
    {
        Append,
        Insert,
        Replace,
        Remove,
        MatchOnly
    }
}
