// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Xml.Linq;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a nonselectable
    /// <see cref="T:System.Windows.Forms.ToolStripItem" />
    /// that renders text and images and can display hyperlinks.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripLabel : WidgetToolStrip<Gtk.MenuItem>
    {
        public ToolStripLabel() : this("", null, null, "")
        {

        }

        public ToolStripLabel(string text) : this(text, null, null, "")
        {

        }

        public ToolStripLabel(string text, Image image) : this(text, image, null, "")
        {
        }

        public ToolStripLabel(string text, Image image, EventHandler onClick) : this(text, image, onClick, "")
        {

        }

        public ToolStripLabel(string text, Image image, EventHandler onClick, string name) : base(null, text, image, onClick, name)
        {
        }

    }

}
