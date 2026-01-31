using System;
namespace System.Windows.Forms
{
    //
    // Summary:
    //     Specifies what to render (image or text) for this System.Windows.Forms.ToolStripItem.
    public enum ToolStripItemDisplayStyle
    {
        //
        // Summary:
        //     Specifies that neither image nor text is to be rendered for this System.Windows.Forms.ToolStripItem.
        None = 0,
        //
        // Summary:
        //     Specifies that only text is to be rendered for this System.Windows.Forms.ToolStripItem.
        Text = 1,
        //
        // Summary:
        //     Specifies that only an image is to be rendered for this System.Windows.Forms.ToolStripItem.
        Image = 2,
        //
        // Summary:
        //     Specifies that both an image and text are to be rendered for this System.Windows.Forms.ToolStripItem.
        ImageAndText = 3
    }
}