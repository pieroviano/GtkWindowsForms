using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{

    /// <summary>
    /// Specifies how tabs in a tab control are sized.
    /// </summary>
    public enum TabSizeMode
    {

        /// <summary>
        /// The width of each tab is sized to accommodate what is displayed on the tab, and
        /// the size of tabs in a row are not adjusted to fill the entire width of the container
        /// control.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// The width of each tab is sized so that each row of tabs fills the entire width
        /// of the container control. This is only applicable to tab controls with more than
        /// one row.
        /// </summary>
        FillToRight = 1,

        /// <summary>
        /// All tabs in a control are the same width.
        /// </summary>
        Fixed = 2
    }
}