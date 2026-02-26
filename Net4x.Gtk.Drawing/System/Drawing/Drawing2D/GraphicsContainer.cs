namespace System.Drawing.Drawing2D
{
    /// <summary>
    /// Represents the internal data of a graphics container. This class is used when saving the state of a
    /// <see cref="System.Drawing.Graphics" /> object using the
    /// <see cref="System.Drawing.Graphics.BeginContainer" />
    /// and
    /// <see cref="System.Drawing.Graphics.EndContainer(System.Drawing.Drawing2D.GraphicsContainer)" /> methods. This class cannot be inherited.
    /// </summary>
    public sealed class GraphicsContainer : MarshalByRefObject
    {
        internal GraphicsContainer()
        {
        }
    }
}
