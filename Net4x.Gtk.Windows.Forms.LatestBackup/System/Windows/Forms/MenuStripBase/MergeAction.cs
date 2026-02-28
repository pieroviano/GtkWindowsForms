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
        /// <summary>
        /// Appends the item to the end of the collection, ignoring match results.
        /// </summary>
        Append,
        /// <summary>
        /// Inserts the item to the target's collection immediately preceding the matched item. A match of the end of the list results in the item being appended to the list. If there is no match or the match is at the beginning of the list, the item is inserted at the beginning of the collection.
        /// </summary>
        Insert,
        /// <summary>
        /// Replaces the matched item with the source item. The original item's drop-down items do not become children of the incoming item.
        /// </summary>
        Replace,
        /// <summary>
        /// Removes the matched item.
        /// </summary>
        Remove,
        /// <summary>
        /// A match is required, but no action is taken. Use this for tree creation and successful access to nested layouts.
        /// </summary>
        MatchOnly
    }
}
