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
    public readonly LinkLabelBase self = new();

    /// <summary>
    /// Occurs when the control is clicked.
    /// </summary>
    public override event EventHandler? Click;

    /// <summary>
    /// Occurs when link clicked.
    /// </summary>
    public virtual event LinkLabelLinkClickedEventHandler? LinkClicked;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.LinkLabel" /> class.
    /// </summary>
    public LinkLabel() : base()
    {
        self.Override.sender = this;
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
    // This was an auto-property nothing ever assigned, so linkLabel.Links was null and every use of
    // it threw a NullReferenceException before it even reached the unimplemented members.
    public LinkCollection Links => _links ??= new LinkCollection(this);

    private LinkCollection? _links;

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
        get => self.Visited;
        set => self.Visited = value;
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
    protected internal override void OnClick(System.EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the LinkClicked event.
    /// </summary>
    protected internal virtual void OnLinkClicked(System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
    {
        LinkClicked?.Invoke(this, e);
    }

    /// <summary>
    /// Handles GTK's activate-link signal. It funnels through <see cref="OnLinkClicked" /> so an override
    /// there runs — and can suppress the event by not calling base — exactly as in classic WinForms.
    /// Raising <see cref="LinkClicked" /> directly here bypassed that override, so it never ran.
    /// </summary>
    internal void LinkLabel_ActivateLink(object o, Gtk.ActivateLinkArgs args)
    {
        OnLinkClicked(new LinkLabelLinkClickedEventArgs(new Link() { Description = self.Label, LinkData = self.Uri }));
    }

    private void LinkLabel_Click(object sender, EventArgs e)
    {
        OnClick(e);
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
        /// <see cref="T:System.Windows.Forms.LinkLabel.Link" /> class covering the given span of text.
        /// </summary>
        /// <param name="start">The character position the link starts at.</param>
        /// <param name="length">The number of characters the link covers.</param>
        /// <remarks>
        /// This constructor and the one below used to have empty bodies, so the span they were given was
        /// discarded and there were no Start/Length properties to read it back from.
        /// </remarks>
        public Link(int start, int length)
        {
            Start = start;
            Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Forms.LinkLabel.Link" /> class covering the given span of text and
        /// carrying the given data.
        /// </summary>
        /// <param name="start">The character position the link starts at.</param>
        /// <param name="length">The number of characters the link covers.</param>
        /// <param name="linkData">The data associated with the link.</param>
        public Link(int start, int length, object linkData) : this(start, length)
        {
            LinkData = linkData;
        }

        /// <summary>
        /// Gets or sets the character position in the label text at which the link starts.
        /// </summary>
        public int Start
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of characters of the label text the link covers.
        /// </summary>
        public int Length
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the link is enabled.
        /// </summary>
        [DefaultValue(true)]
        public bool Enabled
        {
            get;
            set;
        } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the link has been visited.
        /// </summary>
        [DefaultValue(false)]
        public bool Visited
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the link.
        /// </summary>
        public string Name
        {
            get;
            set;
        } = string.Empty;

        /// <summary>
        /// Gets or sets user-defined data associated with the link.
        /// </summary>
        public object? Tag
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description of the link.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string? Description
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
        public object? LinkData
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
        private readonly List<Link> items = new();

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
        /// <remarks>
        /// The collection had no backing store at all: Add overwrote the owner's Text with the value's
        /// string form and returned a hard-coded 1, Clear wiped the Text, Contains did a substring search
        /// on it, and every other member threw. It is now a real ordered collection of links.
        /// </remarks>
        public LinkCollection(LinkLabel owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        /// <summary>
        /// Gets the number of links in the collection.
        /// </summary>
        public int Count => items.Count;

        /// <summary>
        /// Gets a value indicating whether the collection has a fixed size.
        /// </summary>
        public bool IsFixedSize => false;

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Gets a value indicating whether access to the collection is synchronised.
        /// </summary>
        public bool IsSynchronized => false;

        /// <summary>
        /// Gets an object that can be used to synchronise access to the collection.
        /// </summary>
        public object SyncRoot => ((ICollection)items).SyncRoot;

        /// <summary>
        /// Gets a value indicating whether the collection holds no links.
        /// </summary>
        public bool LinksAdded => items.Count > 0;

        /// <summary>
        /// Gets or sets the link at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the link.</param>
        public Link this[int index]
        {
            get => items[index];
            set => items[index] = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// Gets the link with the specified name, or <see langword="null" /> when there is none.
        /// </summary>
        /// <param name="key">The name of the link.</param>
        public virtual Link? this[string key]
        {
            get
            {
                var index = IndexOfKey(key);

                return index == -1 ? null : items[index];
            }
        }

        object? IList.this[int index]
        {
            get => items[index];
            set => items[index] = value as Link ?? throw new ArgumentException("Value must be a Link.", nameof(value));
        }

        /// <summary>
        /// Adds a link covering the given span of the label text.
        /// </summary>
        /// <param name="start">The character position the link starts at.</param>
        /// <param name="length">The number of characters the link covers.</param>
        /// <returns>The link that was added.</returns>
        public Link Add(int start, int length) => Add(new Link(start, length));

        /// <summary>
        /// Adds a link covering the given span of the label text and carrying the given data.
        /// </summary>
        /// <param name="start">The character position the link starts at.</param>
        /// <param name="length">The number of characters the link covers.</param>
        /// <param name="linkData">The data associated with the link.</param>
        /// <returns>The link that was added.</returns>
        public Link Add(int start, int length, object linkData) => Add(new Link(start, length, linkData));

        /// <summary>
        /// Adds the specified link to the collection.
        /// </summary>
        /// <param name="value">The link to add.</param>
        /// <returns>The link that was added.</returns>
        public Link Add(Link value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            items.Add(value);

            return value;
        }

        int IList.Add(object? value)
        {
            if (value is not Link link)
            {
                throw new ArgumentException("Value must be a Link.", nameof(value));
            }

            items.Add(link);

            return items.Count - 1;
        }

        /// <summary>
        /// Removes every link from the collection.
        /// </summary>
        public void Clear() => items.Clear();

        /// <summary>
        /// Indicates whether the specified link is in the collection.
        /// </summary>
        /// <param name="link">The link to look for.</param>
        public bool Contains(Link link) => items.Contains(link);

        bool IList.Contains(object? value) => value is Link link && items.Contains(link);

        /// <summary>
        /// Indicates whether a link with the specified name is in the collection.
        /// </summary>
        /// <param name="key">The name to look for.</param>
        public virtual bool ContainsKey(string? key) => IndexOfKey(key) != -1;

        /// <summary>
        /// Copies the links into the given array starting at the given index.
        /// </summary>
        /// <param name="array">The array to copy into.</param>
        /// <param name="index">The position in the array to start at.</param>
        public void CopyTo(Array array, int index) => ((ICollection)items).CopyTo(array, index);

        /// <summary>
        /// Returns an enumerator over the links.
        /// </summary>
        public IEnumerator GetEnumerator() => items.GetEnumerator();

        /// <summary>
        /// Returns the index of the specified link, or -1 when it is not in the collection.
        /// </summary>
        /// <param name="link">The link to locate.</param>
        public int IndexOf(Link link) => items.IndexOf(link);

        int IList.IndexOf(object? value) => value is Link link ? items.IndexOf(link) : -1;

        /// <summary>
        /// Returns the index of the link with the specified name, or -1 when there is none.
        /// </summary>
        /// <param name="key">The name to look for.</param>
        public virtual int IndexOfKey(string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }

            return items.FindIndex(link => string.Equals(link.Name, key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Inserts a link at the specified position.
        /// </summary>
        /// <param name="index">The position to insert at.</param>
        /// <param name="link">The link to insert.</param>
        public void Insert(int index, Link link)
        {
            if (link == null)
            {
                throw new ArgumentNullException(nameof(link));
            }

            items.Insert(index, link);
        }

        void IList.Insert(int index, object? value)
        {
            if (value is not Link link)
            {
                throw new ArgumentException("Value must be a Link.", nameof(value));
            }

            items.Insert(index, link);
        }

        /// <summary>
        /// Removes the specified link from the collection.
        /// </summary>
        /// <param name="value">The link to remove.</param>
        public void Remove(Link value) => items.Remove(value);

        void IList.Remove(object? value)
        {
            if (value is Link link)
            {
                items.Remove(link);
            }
        }

        /// <summary>
        /// Removes the link at the specified position.
        /// </summary>
        /// <param name="index">The position of the link to remove.</param>
        public void RemoveAt(int index) => items.RemoveAt(index);

        /// <summary>
        /// Removes the link with the specified name, if there is one.
        /// </summary>
        /// <param name="key">The name of the link to remove.</param>
        public virtual void RemoveByKey(string? key)
        {
            var index = IndexOfKey(key);
            if (index != -1)
            {
                items.RemoveAt(index);
            }
        }
    }
}
