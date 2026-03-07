// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a dialog box form that contains a
    /// <see cref="System.Windows.Forms.PrintPreviewControl" />
    /// for printing from a Windows Forms application.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DesignTimeVisible(true)]
    //[DefaultProperty(nameof(Document))]
    [ToolboxItemFilter("System.Windows.Forms.Control.TopLevel")]
    [ToolboxItem(true)]
    public partial class PrintPreviewDialog : ScrollableControl
    {

        public ViewportBase self = new ViewportBase();

        private PrintPreviewControl _previewControl;

        private Gtk.Box box;

        private Form previewForm;

        public event EventHandler MaximumSizeChanged;

        public event EventHandler MinimumSizeChanged;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.PrintPreviewDialog" /> class.
        /// </summary>
        public PrintPreviewDialog() : base()
        {

        }

        /// <summary>
        /// Gets or sets the button on the form that is clicked when the user presses the ENTER key.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Windows.Forms.IButtonControl" /> that represents the button to use as the accept button for the form.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IButtonControl AcceptButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the form adjusts its size to fit the height of the font used on the form and scales its controls.
        /// </summary>
        /// <returns>true if the form will automatically scale itself and its controls based on the current font assigned to the form; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool AutoScale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the cancel button for the
        /// <see cref="System.Windows.Forms.PrintPreviewDialog" />
        /// </summary>
        /// <returns>Allows a control to act like a button on a form.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public IButtonControl CancelButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether a control box is displayed in the caption bar of the form.
        /// </summary>
        /// <returns>true if the form displays a control box in the upper-left corner of the form; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool ControlBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document to preview.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Drawing.Printing.PrintDocument" /> representing the document to preview.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public PrintDocument Document
        {
            get;
            set;
        }

        public override object GtkControl { get => self; }

        /// <summary>
        /// Gets or sets a value indicating whether a Help button should be displayed in the caption box of the form.
        /// </summary>
        /// <returns>true to display a Help button in the form's caption bar; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool HelpButton
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the icon for the form.
        /// </summary>
        /// <returns>An
        /// <see cref="System.Drawing.Icon" /> that represents the icon for the form.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public System.Drawing.Icon Icon
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the form is a container for multiple document interface (MDI) child forms.
        /// </summary>
        /// <returns>true if the form is a container for MDI child forms; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool IsMdiContainer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the form will receive key events before the event is passed to the control that has focus.
        /// </summary>
        /// <returns>true if the form will receive all key events; false if the currently selected control on the form receives key events. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool KeyPreview
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating the
        /// <see cref="System.Windows.Forms.PrintPreviewControl" /> contained in this form.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Windows.Forms.PrintPreviewControl" /> contained in this form.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public PrintPreviewControl PrintPreviewControl => _previewControl;

        /// <summary>
        /// Gets or sets the style of the size grip to display in the lower-right corner of the form.
        /// </summary>
        /// <returns>Gets or sets the style of the size grip to display in the lower-right corner of the form.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public SizeGripStyle SizeGripStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the starting position of the dialog box at run time.
        /// </summary>
        /// <returns>A
        /// <see cref="System.Windows.Forms.FormStartPosition" /> that represents the starting position of the dialog box.
        /// </returns>
        /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The value specified is outside the range of valid values.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        public FormStartPosition StartPosition
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the form should be displayed as the topmost form of your application.
        /// </summary>
        /// <returns>true to display the form as a topmost form; otherwise, false. The default is false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public bool TopMost
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether printing uses the anti-aliasing features of the operating system.
        /// </summary>
        /// <returns>true if anti-aliasing is used; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public bool UseAntiAlias
        {
            get => PrintPreviewControl.UseAntiAlias;
            set => PrintPreviewControl.UseAntiAlias = value;
        }

        public override void Show()
        {
            this.Show(null);
        }

        public void Show(IWin32Window owner)
        {
            if (owner == this)
            {
                throw new InvalidOperationException("OwnsSelfOrOwner");
            }
            Init();
            previewForm.Show(owner);
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            Init();

            return previewForm.ShowDialog(owner);
        }

        public DialogResult ShowDialog()
        {
            return ShowDialog(null);
        }

        private void Init()
        {
            previewForm = new Form();
            previewForm.AutoScroll = true;
            previewForm.self.Resizable = true;
            _previewControl = new PrintPreviewControl();
            //document size(794,1123)px
            _previewControl.self.MarginBottom = 20;
            _previewControl.Zoom = 1;
            _previewControl.AutoZoom = true;
            _previewControl.Document = Document;
            int formwidth = _previewControl.Width + 100;
            int formheight = _previewControl.Height + 100;
            previewForm.ClientSize = new Size(formwidth, formheight);
            box = new Gtk.Box(Gtk.Orientation.Vertical, 15);
            Gtk.Box header = new Gtk.Box(Gtk.Orientation.Horizontal, 20);
            header.PackStart(new Gtk.Label(System.Properties.Resources.Gtk_Print_preview), false, false, 0);
            Gtk.Button printbutton = new Gtk.Button(System.Properties.Resources.Gtk_Print) { WidthRequest = 200 };
            printbutton.ButtonReleaseEvent += Printbutton_ButtonReleaseEvent;
            header.PackEnd(printbutton, false, false, 0);
            box.PackStart(header, false, true, 0);
            box.PackStart(_previewControl.Widget, false, true, 0);
            box.MarginTop = 20;
            box.MarginStart = (previewForm.Width - _previewControl.Width) / 2;
            previewForm.Controls.Add(box);
            previewForm.SizeChanged += PreviewForm_SizeChanged;
        }

        private void PreviewForm_SizeChanged(object sender, EventArgs e)
        {
            box.MarginStart = (previewForm.Width - _previewControl.Width) / 2;
        }

        private void Printbutton_ButtonPressEvent(object o, Gtk.ButtonPressEventArgs args)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = _previewControl.Document;
            printDialog.AllowPrintToFile = false;
            printDialog.PrintToFile = false;
            printDialog.Document.DocumentName = "PrintDoument";
            printDialog.ShowDialog(previewForm);
        }

        private void Printbutton_ButtonReleaseEvent(object o, ButtonReleaseEventArgs args)
        {
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = _previewControl.Document;
            printDialog.AllowPrintToFile = false;
            printDialog.PrintToFile = false;
            printDialog.Document.DocumentName = "PrintDoument";
            printDialog.ShowDialog(previewForm);
        }

    }
}
