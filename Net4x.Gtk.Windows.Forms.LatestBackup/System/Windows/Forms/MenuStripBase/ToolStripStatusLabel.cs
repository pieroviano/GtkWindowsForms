using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a panel in a
    /// <see cref="System.Windows.Forms.StatusStrip" /> control.
    /// </summary>
    public class ToolStripStatusLabel : ToolStripLabel
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> class.
        /// </summary>
        public ToolStripStatusLabel()
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> class that displays the specified text.
        /// </summary>
        /// <param name="text">A
        /// <see cref="System.String" /> representing the text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        public ToolStripStatusLabel(string text)
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> class that displays the specified image.
        /// </summary>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> that is displayed on the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        public ToolStripStatusLabel(Image image)
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> class that displays the specified image and text.
        /// </summary>
        /// <param name="text">A
        /// <see cref="System.String" /> representing the text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> that is displayed on the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        public ToolStripStatusLabel(string text, Image image)
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> class that displays the specified image and text, and that carries out the specified action when the user clicks the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </summary>
        /// <param name="text">A
        /// <see cref="System.String" /> representing the text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> that is displayed on the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        /// <param name="onClick">Specifies the action to carry out when the control is clicked.
        /// </param>
        public ToolStripStatusLabel(string text, Image image, EventHandler onClick)
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> class with the specified name that displays the specified image and text, and that carries out the specified action when the user clicks the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </summary>
        /// <param name="text">A
        /// <see cref="System.String" /> representing the text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        /// <param name="image">An
        /// <see cref="System.Drawing.Image" /> that is displayed on the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        /// <param name="onClick">Specifies the action to carry out when the control is clicked.
        /// </param>
        /// <param name="name">The name of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </param>
        public ToolStripStatusLabel(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
        }
        /// <summary>
        /// Gets or sets a value that indicates which sides of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> show borders.
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabelBorderSides" /> values. The default is
        /// <see cref="System.Windows.Forms.ToolStripStatusLabelBorderSides.None" />
        /// </returns>
        [DefaultValue(ToolStripStatusLabelBorderSides.None)]
        public ToolStripStatusLabelBorderSides BorderSides { get; set; }

        /// <summary>
        /// Gets or sets the border style of the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.Border3DStyle" /> values. The default is
        /// <see cref="System.Windows.Forms.Border3DStyle.Flat" />
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value of
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel.BorderStyle" /> is not one of the
        /// <see cref="System.Windows.Forms.Border3DStyle" /> values.
        /// </exception>
        [DefaultValue(Border3DStyle.Flat)]
        public Border3DStyle BorderStyle { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> automatically fills the available space on the
        /// <see cref="System.Windows.Forms.StatusStrip" /> as the form is resized.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Windows.Forms.ToolStripStatusLabel" /> automatically fills the available space on the
        /// <see cref="System.Windows.Forms.StatusStrip" /> as the form is resized; otherwise, false. The default is false.
        /// </returns>
        [DefaultValue(false)]
        public bool Spring { get; set; }
    }
}
