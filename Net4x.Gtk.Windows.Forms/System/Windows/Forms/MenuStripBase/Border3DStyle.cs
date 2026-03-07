namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the style of a three-dimensional border.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum Border3DStyle
    {
        /// <summary>
        /// The border is drawn outside the specified rectangle, preserving the dimensions of the rectangle for drawing.
        /// </summary>
        Adjust = 0x2000,
        /// <summary>
        /// The inner and outer edges of the border have a raised appearance.
        /// </summary>
        Bump = 9,
        /// <summary>
        /// The inner and outer edges of the border have an etched appearance.
        /// </summary>
        Etched = 6,
        /// <summary>
        /// The border has no three-dimensional effects.
        /// </summary>
        Flat = 16394,
        /// <summary>
        /// The border has raised inner and outer edges.
        /// </summary>
        Raised = 5,
        /// <summary>
        /// The border has a raised inner edge and no outer edge.
        /// </summary>
        RaisedInner = 4,
        /// <summary>
        /// The border has a raised outer edge and no inner edge.
        /// </summary>
        RaisedOuter = 1,
        /// <summary>
        /// The border has sunken inner and outer edges.
        /// </summary>
        Sunken = 10,
        /// <summary>
        /// The border has a sunken inner edge and no outer edge.
        /// </summary>
        SunkenInner = 8,
        /// <summary>
        /// The border has a sunken outer edge and no inner edge.
        /// </summary>
        SunkenOuter = 2
    }
}
