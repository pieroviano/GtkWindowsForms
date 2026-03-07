/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a Windows label control that can display hyperlinks.
    /// </summary>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    [DesignerCategory("Component")]
    public partial class LinkLabel : Control
    {

        public readonly LinkLabelBase self = new LinkLabelBase();

        public override event EventHandler Click;

        public event LinkLabelLinkClickedEventHandler LinkClicked;
        /// <summary>
        /// Initializes a new default instance of the
        /// <see cref="System.Windows.Forms.LinkLabel" /> class.
        /// </summary>
        public LinkLabel() : base()
        {
            self.Clicked += LinkLabel_Click;
            self.ActivateLink += LinkLabel_ActivateLink;
        }
        /// <summary>
        /// Gets or sets the color used to display an active link.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the color to display an active link. The default color is specified by the system, typically this color is Color.Red.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color ActiveLinkColor { get; set; }

        /// <summary>
        /// Gets or sets the color used when displaying a disabled link.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the color when displaying a disabled link. The default is Empty.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color DisabledLinkColor { get; set; }

        /// <summary>
        /// Gets or sets the flat style appearance of the
        /// <see cref="System.Windows.Forms.LinkLabel" />
        /// </summary>
        /// <returns>One of the
        /// <see cref="System.Windows.Forms.FlatStyle" /> values.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        //public LinkBehavior LinkBehavior { get; set; }
        //public LinkArea LinkArea { get; set; }
        public FlatStyle FlatStyle { get; set; }

        public override object GtkControl => self;

        /// <summary>
        /// Gets or sets the color used when displaying a normal link.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the color used to displaying a normal link. The default color is specified by the system, typically this color is Color.Blue.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color LinkColor { get; set; }

        /// <summary>
        /// Gets the collection of links contained within the
        /// <see cref="System.Windows.Forms.LinkLabel" />
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.LinkLabel.LinkCollection" /> that represents the links contained within the
        /// <see cref="System.Windows.Forms.LinkLabel" /> control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public LinkCollection Links { get; }

        /// <summary>
        /// Gets or sets a value indicating whether a link should be displayed as though it were visited.
        /// </summary>
        /// <returns>true if links should display as though they were visited; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool LinkVisited { get; set; }

        /// <summary>
        /// Gets or sets the text displayed by the
        /// <see cref="System.Windows.Forms.LinkLabel" />
        /// </summary>
        /// <returns>The text displayed by the
        /// <see cref="System.Windows.Forms.LinkLabel" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public override string Text { get { return string.IsNullOrEmpty(self.Label) ? self.Uri : self.Label; } set { self.Label = value; self.Uri = value; } }

        /// <summary>
        /// Gets or sets a value that determines whether to use the
        /// <see cref="System.Drawing.Graphics" /> class (GDI+) or the
        /// <see cref="System.Windows.Forms.TextRenderer" /> class (GDI) to render text.
        /// </summary>
        /// <returns>true if the
        /// <see cref="System.Drawing.Graphics" /> class should be used to perform text rendering for compatibility with versions 1.0 and 1.1. of the .NET Framework; otherwise, false. The default is false.
        /// </returns>
        public bool UseCompatibleTextRendering { get; set; }

        /// <summary>
        /// Gets or sets the color used when displaying a link that that has been previously visited.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Drawing.Color" /> that represents the color used to display links that have been visited. The default color is specified by the system, typically this color is Color.Purple.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public Color VisitedLinkColor { get; set; }

        private void LinkLabel_ActivateLink(object o, Gtk.ActivateLinkArgs args)
        {
            if (LinkClicked != null)
            {
                LinkClicked(this, new LinkLabelLinkClickedEventArgs(new Link() { Description = self.Label, LinkData = self.Uri }));
            }
        }

        private void LinkLabel_Click(object sender, EventArgs e)
        {
            if (Click != null)
            {
                Click(this, e);
            }
        }
        /// <summary>
        /// Represents a link within a
        /// <see cref="System.Windows.Forms.LinkLabel" /> control.
        /// </summary>
        public class Link
        {
            public Link()
            {
            }

            public Link(int start, int length)
            {

            }

            public Link(int start, int length, object linkData)
            {

            }

            public string Description { get; set; }

            [DefaultValue(null)]
            public object LinkData { get; set; }
        }

        /// <summary>
        /// Represents the collection of links within a
        /// <see cref="System.Windows.Forms.LinkLabel" /> control.
        /// </summary>
        public class LinkCollection : IList
        {
            private int lastAccessedIndex = -1;

            private bool linksAdded = false;

            private readonly LinkLabel owner;

            public LinkCollection(LinkLabel owner)
            {
                this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            public int Count => throw new NotImplementedException();

            public bool IsFixedSize => throw new NotImplementedException();

            public bool IsReadOnly => throw new NotImplementedException();

            public bool IsSynchronized => throw new NotImplementedException();

            public object SyncRoot => throw new NotImplementedException();

            public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public int Add(object value)
            {
                if (value is Label)
                {
                    owner.Text = ((Label)value).Text;
                }
                else
                {
                    owner.Text = value?.ToString();
                }

                return 1;
            }

            public void Clear()
            {
                owner.Text = "";
            }

            public bool Contains(object value)
            {
                return owner.Text.Contains(value.ToString());
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public int IndexOf(object value)
            {
                throw new NotImplementedException();
            }

            public void Insert(int index, object value)
            {
                throw new NotImplementedException();
            }

            public void Remove(object value)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }
        }
    }
}
