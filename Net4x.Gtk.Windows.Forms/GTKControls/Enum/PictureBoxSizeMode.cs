using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    public enum PictureBoxSizeMode
    {
        //
        // Summary:
        //     The image is placed in the upper-left corner of the System.Windows.Forms.PictureBox.
        //     The image is clipped if it is larger than the System.Windows.Forms.PictureBox
        //     it is contained in.
        Normal = 0,
        //
        // Summary:
        //     The image within the System.Windows.Forms.PictureBox is stretched or shrunk to
        //     fit the size of the System.Windows.Forms.PictureBox.
        StretchImage = 1,
        //
        // Summary:
        //     The System.Windows.Forms.PictureBox is sized equal to the size of the image that
        //     it contains.
        AutoSize = 2,
        //
        // Summary:
        //     The image is displayed in the center if the System.Windows.Forms.PictureBox is
        //     larger than the image. If the image is larger than the System.Windows.Forms.PictureBox,
        //     the picture is placed in the center of the System.Windows.Forms.PictureBox and
        //     the outside edges are clipped.
        CenterImage = 3,
        //
        // Summary:
        //     The size of the image is increased or decreased maintaining the size ratio.
        Zoom = 4
    }
}
