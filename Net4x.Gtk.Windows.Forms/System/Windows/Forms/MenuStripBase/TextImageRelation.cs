namespace System.Windows.Forms;

/// <summary>
/// Specifies the position of the text and image relative to each other on a control.
/// </summary>
public enum TextImageRelation
{
    /// <summary>
    /// The overlay
    /// </summary>
    Overlay = 0,
    /// <summary>
    /// The image before text
    /// </summary>
    ImageBeforeText = 4,
    /// <summary>
    /// The text before image
    /// </summary>
    TextBeforeImage = 8,
    /// <summary>
    /// The image above text
    /// </summary>
    ImageAboveText = 1,
    /// <summary>
    /// The text above image
    /// </summary>
    TextAboveImage = 2
}
