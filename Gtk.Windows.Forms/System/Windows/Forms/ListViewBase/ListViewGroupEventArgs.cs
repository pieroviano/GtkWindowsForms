using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

/// <summary>
/// Class ListViewGroupEventArgs. Implements the
/// <see cref="T:System.EventArgs" />
/// </summary>
/// <seealso cref="T:System.EventArgs" />
public class ListViewGroupEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ListViewGroupEventArgs" /> class.
    /// </summary>
    /// <param name="groupIndex">
    /// Index of the group.
    /// </param>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public ListViewGroupEventArgs(int groupIndex)
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Gets the index of the group.
    /// </summary>
    /// <value>
    /// The index of the group.
    /// </value>
    /// <exception cref="T:System.NotImplementedException">
    /// </exception>
    public int GroupIndex
    {
        [CompilerGenerated]
        get
        {
            throw new System.NotImplementedException();
        }
    }
}