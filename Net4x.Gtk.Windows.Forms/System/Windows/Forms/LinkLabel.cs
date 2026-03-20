/*
 * Based on GTK components, it is a cross-platform interface component that is compatible with the native C# control winform interface.
 * Use this component GtkSystem.Windows.Forms instead of Microsoft.WindowsDesktop.App.WindowsForms, compile once, and run across Windows, Linux, and macOS
 * Technical support 438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory438865652@qq.com，https://www.gtkapp.com, https://gitee.com/easywebfactory, https://github.com/easywebfactory
 * author:chenhongjin
 */
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using GtkSystem.Windows.Forms.GtkControls.ControlBase;

namespace System.Windows.Forms;

/// <summary>
/// Represents a Windows label control that can display hyperlinks.
/// </summary>
[DesignerCategory("Component")]
public partial class LinkLabel : Control
{
    /// <summary>
    /// The self
    /// </summary>
    public readonly LinkLabelBase self = new LinkLabelBase();

    /// <summary>
    /// Occurs when the control is clicked.
    /// </summary>
    public override event EventHandler Click;

    /// <summary>
    /// Occurs when link clicked.
    /// </summary>
    public virtual event LinkLabelLinkClickedEventHandler LinkClicked;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.LinkLabel" /> class.
    /// </summary>
    public LinkLabel() : base()
    {
        self.Clicked += LinkLabel_Click;
        self.ActivateLink += LinkLabel_ActivateLink;
    }

    /// <summary>
    /// Gets or sets the color of the active link.
    /// </summary>
    /// <value>
    /// The color of the active link.
    /// </value>
    public Color ActiveLinkColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the disabled link.
    /// </summary>
    /// <value>
    /// The color of the disabled link.
    /// </value>
    public Color DisabledLinkColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the flat style.
    /// </summary>
    /// <value>
    /// The flat style.
    /// </value>
    //public LinkBehavior LinkBehavior { get; set; }
    //public LinkArea LinkArea { get; set; }
    public FlatStyle FlatStyle
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the GTK control.
    /// </summary>
    /// <value>
    /// The GTK control.
    /// </value>
    public override object GtkControl => self;

    /// <summary>
    /// Gets or sets the color of the link.
    /// </summary>
    /// <value>
    /// The color of the link.
    /// </value>
    public Color LinkColor
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the links.
    /// </summary>
    /// <value>
    /// The links.
    /// </value>
    public LinkCollection Links { get; }

    /// <summary>
    /// Gets or sets a value indicating whether link visited.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if link visited; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool LinkVisited
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>
    /// The text.
    /// </value>
    public override string Text
    {
        get { return string.IsNullOrEmpty(self.Label) ? self.Uri : self.Label; }
        set { self.Label = value; self.Uri = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether use compatible text rendering.
    /// </summary>
    /// <value>
    /// <c>
    /// true
    /// </c> if use compatible text rendering; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </value>
    public bool UseCompatibleTextRendering
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the color of the visited link.
    /// </summary>
    /// <value>
    /// The color of the visited link.
    /// </value>
    public Color VisitedLinkColor
    {
        get;
        set;
    }

    /// <summary>
    /// Raises the Click event.
    /// </summary>
    protected override void OnClick(System.EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the LinkClicked event.
    /// </summary>
    protected virtual void OnLinkClicked(System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
    {
        LinkClicked?.Invoke(this, e);
    }

    private void LinkLabel_ActivateLink(object o, Gtk.ActivateLinkArgs args)
    {
        LinkClicked?.Invoke(this, new LinkLabelLinkClickedEventArgs(new Link() { Description = self.Label, LinkData = self.Uri }));
    }

    private void LinkLabel_Click(object sender, EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Class Link.
    /// </summary>
    public class Link
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.LinkLabel.Link" /> class.
        /// </summary>
        public Link()
        {
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.LinkLabel.Link" /> class.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        public Link(int start, int length)
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.LinkLabel.Link" /> class.
        /// </summary>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="linkData">
        /// The link data.
        /// </param>
        public Link(int start, int length, object linkData)
        {

        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the link data.
        /// </summary>
        /// <value>
        /// The link data.
        /// </value>
        [DefaultValue(null)]
        public object LinkData
        {
            get;
            set;
        }

    }

    /// <summary>
    /// Class LinkCollection. Implements the
    /// <see cref="T:System.Collections.IList" />
    /// </summary>
    /// <seealso cref="T:System.Collections.IList" />
    public class LinkCollection : IList
    {
        private readonly int lastAccessedIndex = -1;

        private readonly bool linksAdded = false;

        private readonly LinkLabel owner;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.LinkLabel.LinkCollection" /> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        /// owner
        /// </exception>
        public LinkCollection(LinkLabel owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        /// <summary>
        /// Gets the number of elements contained in the
        /// <see cref="T:System.Collections.ICollection">
        /// </see>.
        /// </summary>
        /// <value>
        /// The count.
        /// </value>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public int Count => throw new NotImplementedException();

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="T:System.Collections.IList">
        /// </see> has a fixed size.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is fixed size; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public bool IsFixedSize => throw new NotImplementedException();

        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="T:System.Collections.IList">
        /// </see> is read-only.
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is read only; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public bool IsReadOnly => throw new NotImplementedException();

        /// <summary>
        /// Gets a value indicating whether access to the
        /// <see cref="T:System.Collections.ICollection">
        /// </see> is synchronized (thread safe).
        /// </summary>
        /// <value>
        /// <c>
        /// true
        /// </c> if this instance is synchronized; otherwise,
        /// <c>
        /// false
        /// </c>.
        /// </value>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public bool IsSynchronized => throw new NotImplementedException();

        /// <summary>
        /// Gets an object that can be used to synchronize access to the
        /// <see cref="T:System.Collections.ICollection">
        /// </see>.
        /// </summary>
        /// <value>
        /// The synchronize root.
        /// </value>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public object SyncRoot => throw new NotImplementedException();

        /// <summary>
        /// Gets or sets the
        /// <see cref="T:System.Object" /> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// System.Object.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Adds an item to the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        /// <param name="value">
        /// The object to add to the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </param>
        /// <returns>
        /// The position into which the new element was inserted, or -1 to indicate that the item was not inserted into the collection.
        /// </returns>
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

        /// <summary>
        /// Removes all items from the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        public void Clear()
        {
            owner.Text = "";
        }

        /// <summary>
        /// Determines whether the
        /// <see cref="T:System.Collections.IList">
        /// </see> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The object to locate in the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </param>
        /// <returns>
        /// true if the
        /// <see cref="T:System.Object">
        /// </see> is found in the
        /// <see cref="T:System.Collections.IList">
        /// </see>; otherwise, false.
        /// </returns>
        public bool Contains(object value)
        {
            return owner.Text.Contains(value.ToString());
        }

        /// <summary>
        /// Copies the elements of the
        /// <see cref="T:System.Collections.ICollection">
        /// </see> to an
        /// <see cref="T:System.Array">
        /// </see>, starting at a particular
        /// <see cref="T:System.Array">
        /// </see> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional
        /// <see cref="T:System.Array">
        /// </see> that is the destination of the elements copied from
        /// <see cref="T:System.Collections.ICollection">
        /// </see>. The
        /// <see cref="T:System.Array">
        /// </see> must have zero-based indexing.
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins.
        /// </param>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An
        /// <see cref="T:System.Collections.IEnumerator">
        /// </see> object that can be used to iterate through the collection.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines the index of a specific item in the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        /// <param name="value">
        /// The object to locate in the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </param>
        /// <returns>
        /// The index of
        /// <paramref name="value">
        /// value
        /// </paramref> if found in the list; otherwise, -1.
        /// </returns>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inserts an item to the
        /// <see cref="T:System.Collections.IList">
        /// </see> at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which value should be inserted.
        /// </param>
        /// <param name="value">
        /// The object to insert into the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </param>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </summary>
        /// <param name="value">
        /// The object to remove from the
        /// <see cref="T:System.Collections.IList">
        /// </see>.
        /// </param>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the
        /// <see cref="T:System.Collections.IList">
        /// </see> item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to remove.
        /// </param>
        /// <exception cref="T:System.NotImplementedException">
        /// </exception>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

    }
}