using System;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies that
    /// <see cref="System.Windows.Forms.SplitContainer.Panel1" /> ,
    /// <see cref="System.Windows.Forms.SplitContainer.Panel2" />
    /// , or neither panel is fixed.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum FixedPanel
    {
        /// <summary>
        /// Specifies that neither
        /// <see cref="System.Windows.Forms.SplitContainer.Panel1" />,
        /// <see cref="System.Windows.Forms.SplitContainer.Panel2" /> is fixed. A
        /// <see cref="System.Windows.Forms.Control.Resize" /> event affects both panels.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies that
        /// <see cref="System.Windows.Forms.SplitContainer.Panel1" /> is fixed. A
        /// <see cref="System.Windows.Forms.Control.Resize" /> event affects only
        /// <see cref="System.Windows.Forms.SplitContainer.Panel2" />
        /// </summary>
        Panel1 = 1,
        /// <summary>
        /// Specifies that
        /// <see cref="System.Windows.Forms.SplitContainer.Panel2" /> is fixed. A
        /// <see cref="System.Windows.Forms.Control.Resize" /> event affects only
        /// <see cref="System.Windows.Forms.SplitContainer.Panel1" />
        /// </summary>
        Panel2 = 2
    }
}
