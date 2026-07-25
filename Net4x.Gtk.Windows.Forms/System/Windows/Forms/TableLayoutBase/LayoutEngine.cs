using System.Windows.Forms;

namespace System.Windows.Forms.Layout;

/// <summary>
/// Provides the base class for implementing layout engines. This mirrors the real
/// <c>System.Windows.Forms.Layout.LayoutEngine</c> (the correct namespace), replacing the earlier
/// <c>System.Windows.Forms.LayoutEngine</c> placeholder.
/// </summary>
public class LayoutEngine
{
    /// <summary>Initializes the layout for the specified child element.</summary>
    /// <param name="child">The element to initialize.</param>
    /// <param name="specified">A <see cref="T:System.Windows.Forms.BoundsSpecified" /> that indicates which bounds are specified.</param>
    public virtual void InitLayout(object child, BoundsSpecified specified)
    {
    }

    /// <summary>Requests that the layout engine perform a layout operation.</summary>
    /// <param name="container">The container on which the layout is performed.</param>
    /// <param name="layoutEventArgs">A <see cref="T:System.Windows.Forms.LayoutEventArgs" /> that contains the event data.</param>
    /// <returns><see langword="true" /> if layout should be performed again; otherwise <see langword="false" />.</returns>
    public virtual bool Layout(object container, LayoutEventArgs layoutEventArgs)
    {
        return false;
    }
}
