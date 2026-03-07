using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Xml.Linq;
using GLib;
using Pango;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a collection of
    /// <see cref="System.Windows.Forms.ToolStripItem" /> objects.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ToolStripItemCollection : List<ToolStripItem>, ICollection, IEnumerable
    {

        private bool isMenuStrip;

        private bool isStatusStrip;

        private bool isToolStrip;

        private bool isToolStripDropDown;

        private Gtk.Menu menu;

        private ToolStripItem owner;

        private StatusStrip statusStrip;

        private ToolStrip toolStrip;

        public ToolStripItemCollection(ToolStrip owner)
        {
            this.toolStrip = owner;
            isToolStrip = true;
        }

        public ToolStripItemCollection(ToolStrip toolStrip, string owner)
        {
            this.toolStrip = toolStrip;
            isMenuStrip = owner == "MenuStrip";
        }

        public ToolStripItemCollection(StatusStrip owner)
        {
            this.statusStrip = owner;
            isStatusStrip = true;
        }

        public ToolStripItemCollection(ToolStripDropDown owner)
        {
            this.menu = owner.Widget as Gtk.Menu;
            isToolStripDropDown = true;
        }

        public ToolStripItemCollection(ToolStripItem owner)
        {
            this.owner = owner;
            if (owner.Widget is Gtk.Menu gmenu)
            {
                this.menu = gmenu;
                isToolStripDropDown = true;
            }
        }

        public ToolStripItemCollection(ToolStripItem owner, ToolStripItem[] value)
        {
            this.owner = owner;
            isToolStrip = false;
            AddRange(value);
        }
        internal ToolStripItemCollection(ToolStripItem owner, bool itemsCollection)
            : this(owner, itemsCollection, isReadOnly: false)
        {
        }

        internal ToolStripItemCollection(ToolStripItem owner, bool itemsCollection, bool isReadOnly)
        {
            this.owner = owner;
            isToolStrip = false;

        }
        //-------------------
        public new ToolStripItem this[int index]
        {
            get { return base[index]; }
            set { menu.Insert(value.Widget, index); base[index] = value; }
        }
        /// <summary>
        /// Adds a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> that displays the specified text to the collection.
        /// </summary>
        /// <returns>The new
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </returns>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        public ToolStripItem Add(string text)
        {
            return Add(text, null, null);
        }

        /// <summary>
        /// Adds a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> that displays the specified image to the collection.
        /// </summary>
        /// <returns>The new
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </returns>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        public ToolStripItem Add(Image image)
        {
            return Add(null, image, null);
        }

        /// <summary>
        /// Adds a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> that displays the specified image and text to the collection.
        /// </summary>
        /// <returns>The new
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </returns>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        public ToolStripItem Add(string text, Image image)
        {
            return Add(text, image, null);
        }

        /// <summary>
        /// Adds a
        /// <see cref="System.Windows.Forms.ToolStripItem" /> that displays the specified image and text to the collection and that raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event.
        /// </summary>
        /// <returns>The new
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </returns>
        /// <param name="text">The text to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        /// <param name="image">The
        /// <see cref="System.Drawing.Image" /> to be displayed on the
        /// <see cref="System.Windows.Forms.ToolStripItem" />
        /// </param>
        /// <param name="onClick">Raises the
        /// <see cref="System.Windows.Forms.ToolStripItem.Click" /> event.
        /// </param>
        public ToolStripItem Add(string text, Image image, EventHandler onClick)
        {
            ToolStripItem toolStripItem = new ToolStripLabel();
            AddMemu(toolStripItem);

            return toolStripItem;
        }

        public int AddMemu(ToolStripItem item)
        {
            item.Parent = owner;
            if (isToolStrip == true)
            {
                toolStrip.self.Add(item.Widget);
            }
            else if (isStatusStrip == true)
            {
                statusStrip.self.Add(item.Widget);
            }
            else if (isMenuStrip == true)
            {
                toolStrip.self.Add(item.Widget);
            }
            else if (isToolStripDropDown == true)
            {
                menu.Add(item.Widget);
            }
            else
            {
                if (owner.MenuItem.Submenu == null)
                {
                    this.menu = new Gtk.Menu();
                    owner.MenuItem.Submenu = this.menu;
                }
                menu.Add(item.Widget);
            }

            base.Add(item);

            return Count;
        }

        /// <summary>
        /// Adds an array of
        /// <see cref="System.Windows.Forms.ToolStripItem" /> controls to the collection.
        /// </summary>
        /// <param name="toolStripItems">An array of
        /// <see cref="System.Windows.Forms.ToolStripItem" /> controls.
        /// </param>
        /// <exception cref="System.ArgumentNullException">The
        /// <paramref name="toolStripItems" /> parameter is null.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The
        /// <see cref="System.Windows.Forms.ToolStripItemCollection" /> is read-only.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void AddRange(ToolStripItem[] toolStripItems)
        {
            for (int i = 0; i < toolStripItems.Length; i++)
            {
                AddMemu(toolStripItems[i]);
            }
        }

        /// <summary>
        /// Adds a
        /// <see cref="System.Windows.Forms.ToolStripItemCollection" /> to the current collection.
        /// </summary>
        /// <param name="toolStripItems">The
        /// <see cref="System.Windows.Forms.ToolStripItemCollection" /> to be added to the current collection.
        /// </param>
        /// <exception cref="System.ArgumentNullException">The
        /// <paramref name="toolStripItems" /> parameter is null.
        /// </exception>
        /// <exception cref="System.NotSupportedException">The
        /// <see cref="System.Windows.Forms.ToolStripItemCollection" /> is read-only.
        /// </exception>
        /// <filterpriority>1
        /// </filterpriority>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
        /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
        /// </PermissionSet>
        public void AddRange(ToolStripItemCollection toolStripItems)
        {
            int count = toolStripItems.Count;
            for (int i = 0; i < count; i++)
            {
                AddMemu(toolStripItems[i]);
            }
        }
    }
}
