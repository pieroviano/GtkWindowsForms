// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using Gtk;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents the raw preview part of print previewing from a Windows Forms application, without any dialog boxes or buttons. Most
    /// <see cref = "System.Windows.Forms.PrintPreviewControl"/> objects are found on
    /// <see cref = "System.Windows.Forms.PrintPreviewDialog"/>
    /// objects, but they do not have to be.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    [DefaultProperty(nameof(Document))]
    public partial class PrintPreviewControl : Control
    {
        public readonly ViewportBase self = new ViewportBase();
        private bool _autoZoom = true;

        private int _columns = 1;

        private PrintDocument _document;

        private int _rows = 1;

        private int _startPage; // 0-based

        private double _zoom = DefaultZoom;

        // Spacing per page, in mm
        private const int Border = 10;

        private const double DefaultZoom = .3;

        private ViewportBase paper;

        private const double pxscale = 0.75;

        private static readonly object s_startPageChangedEvent = new object();

        private Gtk.ScrolledWindow scroll;

        private const int ScrollSmallChange = 5;

        public event EventHandler StartPageChanged { add => Events.AddHandler(s_startPageChangedEvent, value); remove => Events.RemoveHandler(s_startPageChangedEvent, value); }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TextChanged { add => base.TextChanged += value; remove => base.TextChanged -= value; }
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref = "System.Windows.Forms.PrintPreviewControl"/> class.
        /// </summary>
        public PrintPreviewControl()
        {
            Size = new Size(600, 500);
            self.StyleContext.AddClass("PrintPreviewBack");
            paper = new ViewportBase();
            paper.Valign = Align.Center;
            paper.Halign = Align.Center;
            paper.StyleContext.AddClass("Paper");
            paper.Drawn += paper_Drawn;
            scroll = new Gtk.ScrolledWindow();
            scroll.Child = paper;
            self.Add(scroll);
            self.Realized += Self_Realized;
            ResetBackColor();
            ResetForeColor();
        }

        /// <summary>
        /// Gets or sets a value indicating whether resizing the control or changing the number of pages shown automatically adjusts the
        /// <see cref = "System.Windows.Forms.PrintPreviewControl.Zoom"/> property.
        /// </summary>
        /// <returns>true if the changing the control size or number of pages adjusts the
        /// <see cref = "System.Windows.Forms.PrintPreviewControl.Zoom"/> property; otherwise, false. The default is true.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(true)]
        public bool AutoZoom
        {
            get => _autoZoom;
            set
            {
                if (_autoZoom != value)
                {
                    _autoZoom = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of pages displayed horizontally across the screen.
        /// </summary>
        /// <returns>The number of pages displayed horizontally across the screen. The default is 1.
        /// </returns>
        /// <exception cref = "System.ArgumentOutOfRangeException">The set value is less than 1.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(1)]
        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the document to preview.
        /// </summary>
        /// <returns>The
        /// <see cref = "System.Drawing.Printing.PrintDocument"/> representing the document to preview.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(null)]
        public PrintDocument Document
        {
            get => _document;
            set
            {
                _document = value;
                paper.WidthRequest = (int)Math.Round(_document.PageSetup.GetPaperWidth(Unit.Points) / pxscale, 0);
                paper.HeightRequest = (int)Math.Round(_document.PageSetup.GetPaperHeight(Unit.Points) / pxscale, 0);
                self.WidthRequest = paper.WidthRequest + 100;
                self.HeightRequest = paper.HeightRequest + 100;
                InvalidatePreview();
            }
        }

        public override object GtkControl => self;

        /// <returns>
        /// One of the
        /// <see cref = "System.Windows.Forms.RightToLeft"/> values. The default is
        /// <see cref = "System.Windows.Forms.RightToLeft.Inherit"/>
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [Localizable(true)]
        [AmbientValue(RightToLeft.Inherit)]
        public override RightToLeft RightToLeft
        {
            get => base.RightToLeft;
            set
            {
                base.RightToLeft = value;
                InvalidatePreview();
            }
        }

        /// <summary>
        /// Gets or sets the number of pages displayed vertically down the screen.
        /// </summary>
        /// <returns>The number of pages displayed vertically down the screen. The default is 0.
        /// </returns>
        /// <exception cref = "System.ArgumentOutOfRangeException">The set value is less than 1.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(1)]
        public int Rows
        {
            get => _rows;
            set
            {
                _rows = value;
            }
        }

        /// <summary>
        /// Gets or sets the page number of the upper left page.
        /// </summary>
        /// <returns>The page number of the upper left page. The default is 0.
        /// </returns>
        /// <exception cref = "System.ArgumentOutOfRangeException">The set value is less than 0.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(0)]
        public int StartPage
        {
            get
            {
                int value = _startPage;
                value = Math.Max(value, 0);

                return value;
            }

            set
            {
                int oldValue = StartPage;
                _startPage = value;
                if (oldValue != _startPage)
                {
                    OnStartPageChanged(EventArgs.Empty);
                }
            }
        }

        [DefaultValue(false)]
        public new bool TabStop { get => base.TabStop; set => base.TabStop = value; }

        /// <summary>
        /// Gets or sets the text associated with this control.
        /// </summary>
        /// <returns>The text associated with this control.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        public override string Text { get => base.Text; set => base.Text = value; }

        /// <summary>
        /// Gets or sets a value indicating whether printing uses the anti-aliasing features of the operating system.
        /// </summary>
        /// <returns>true if anti-aliasing is used; otherwise, false.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        [DefaultValue(false)]
        public bool UseAntiAlias { get; set; }

        /// <summary>
        /// Gets or sets a value indicating how large the pages will appear.
        /// </summary>
        /// <returns>A value indicating how large the pages will appear. A value of 1.0 indicates full size.
        /// </returns>
        /// <exception cref = "System.ArgumentException">The value is less than or equal to 0.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [DefaultValue(DefaultZoom)]
        public double Zoom
        {
            get => _zoom;
            set
            {
                _autoZoom = false;
                _zoom = value;
            }
        }
        /// <summary>
        /// Refreshes the preview of the document.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        public void InvalidatePreview()
        {
            paper.QueueDraw();
        }

        /// <summary>
        /// Resets the
        /// <see cref = "System.Windows.Forms.Control.BackColor"/> property to
        /// <see cref = "System.Drawing.SystemColors.AppWorkspace"/>, which is the default color.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetBackColor() => BackColor = SystemColors.AppWorkspace;

        /// <summary>
        /// Resets the foreground color of the
        /// <see cref = "System.Windows.Forms.PrintPreviewControl"/> to
        /// <see cref = "System.Drawing.Color.White"/>, which is the default color.
        /// </summary>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
        /// </PermissionSet>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override void ResetForeColor() => ForeColor = Color.White;

        /// <summary>
        /// Raises the
        /// <see cref = "System.Windows.Forms.PrintPreviewControl.StartPageChanged"/> event.
        /// </summary>
        /// <param name = "e">An
        /// <see cref = "System.EventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnStartPageChanged(EventArgs e)
        {
            ((EventHandler)Events[s_startPageChangedEvent])?.Invoke(this, e);
        }

        private void paper_Drawn(object o, Gtk.DrawnArgs args)
        {
            if (_document != null)
            {
                PageSetup setup = _document.PageSetup;
                _document.OnBeginPrint(new PrintEventArgs());
                var cr = args.Cr;
                int top = (int)Math.Round(setup.GetTopMargin(Unit.Points) / pxscale, 0);
                int left = (int)Math.Round(setup.GetLeftMargin(Unit.Points) / pxscale, 0);
                int right = (int)Math.Round(setup.GetRightMargin(Unit.Points) / pxscale, 0);
                int bottom = (int)Math.Round(setup.GetBottomMargin(Unit.Points) / pxscale, 0);
                int width = (int)Math.Round(setup.GetPaperWidth(Unit.Points) / pxscale, 0); //page<paper
                int height = (int)Math.Round(setup.GetPaperHeight(Unit.Points) / pxscale, 0);
                if (AutoZoom == false)
                {
                    paper.WidthRequest = (int)Math.Round(width * _zoom, 0);
                    paper.HeightRequest = (int)Math.Round(height * _zoom, 0);
                    self.WidthRequest = paper.WidthRequest + 100;
                    self.HeightRequest = paper.HeightRequest + 100;
                    cr.Scale(_zoom, _zoom);
                }

                cr.Translate(0, 0);
                cr.Rectangle(0, 0, width, height);
                cr.SetSourceRGBA(ForeColor.R / 255, ForeColor.G / 255, ForeColor.B / 255, ForeColor.A / 255);
                cr.Fill();
                _document.OnPrintPage(new PrintPageEventArgs(new Drawing.Graphics((Gtk.Widget)o, cr, new Gdk.Rectangle(0, 0, width, height)), new Drawing.Rectangle(left, top, width - left - right, height - top - bottom), new Drawing.Rectangle(0, 0, width, height), _document.DefaultPageSettings));
                _document.OnEndPrint(new PrintEventArgs());
            }
        }

        private void Self_Realized(object? sender, EventArgs e)
        {
            if (AutoZoom == false)
            {
                paper.WidthRequest = (int)Math.Round(_document.PageSetup.GetPaperWidth(Unit.Points) * _zoom / 0.75, 0);
                paper.HeightRequest = (int)Math.Round(_document.PageSetup.GetPaperHeight(Unit.Points) * _zoom / 0.75, 0);
            }
        }
    }
}
