namespace System.Windows.Forms;

/// <summary>
/// Specifies the position of the image on the control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public enum ImageLayout
{

    /// <summary>
    /// The image is left-aligned at the top across the control's client rectangle.
    /// </summary>
    None = 0,

    /// <summary>
    /// The image is tiled across the control's client rectangle.
    /// </summary>
    Tile = 1,

    /// <summary>
    /// The image is centered within the control's client rectangle.
    /// </summary>
    Center = 2,

    /// <summary>
    /// The image is stretched across the control's client rectangle.
    /// </summary>
    Stretch = 3,

    /// <summary>
    /// The image is enlarged within the control's client rectangle.
    /// </summary>
    Zoom = 4
}
