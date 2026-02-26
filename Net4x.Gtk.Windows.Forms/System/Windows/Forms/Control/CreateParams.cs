// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms
{
    /// <summary>
    /// Encapsulates the information needed when creating a control.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class CreateParams
    {
        /// <summary>
        /// Name of the window class to subclass. The default value for this field
        /// is null, indicating that the window is not a subclass of an existing
        /// window class. To subclass an existing window class, store the window
        /// class name in this field. For example, to subclass the standard edit
        /// control, set this field to "EDIT".
        /// </summary>
        public string? ClassName { get; set; }

        /// <summary>
        /// Gets or sets the control's initial text.
        /// </summary>
        /// <returns>The control's initial text.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public string? Caption { get; set; }

        /// <summary>
        /// Gets or sets a bitwise combination of window style values.
        /// </summary>
        /// <returns>A bitwise combination of the window style values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Style { get; set; }

        /// <summary>
        /// Gets or sets a bitwise combination of extended window style values.
        /// </summary>
        /// <returns>A bitwise combination of the extended window style values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int ExStyle { get; set; }

        /// <summary>
        /// Class style bits. This must be a combination of CS_XXX style flags. This
        /// field is ignored if the className field is not null.
        /// </summary>
        public int ClassStyle { get; set; }

        /// <summary>
        /// Gets or sets the initial left position of the control.
        /// </summary>
        /// <returns>The numeric value that represents the initial left position of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int X { get; set; }

        /// <summary>
        /// Gets or sets the top position of the initial location of the control.
        /// </summary>
        /// <returns>The numeric value that represents the top position of the initial location of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Y { get; set; }

        /// <summary>
        /// Gets or sets the initial width of the control.
        /// </summary>
        /// <returns>The numeric value that represents the initial width of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the initial height of the control.
        /// </summary>
        /// <returns>The numeric value that represents the initial height of the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the control's parent.
        /// </summary>
        /// <returns>An
        /// <see cref="System.IntPtr" /> that contains the window handle of the control's parent.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IntPtr Parent { get; set; }

        /// <summary>
        /// Gets or sets additional parameter information needed to create the control.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Object" /> that holds additional parameter information needed to create the control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object? Param { get; set; }
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string ToString()
        {
            return $"CreateParams {{'{ClassName}', '{Caption}', 0x{Style:x}, 0x{ExStyle:x}, {{{X}, {Y}, {Width}, {Height}}}}}";
        }
    }
}
