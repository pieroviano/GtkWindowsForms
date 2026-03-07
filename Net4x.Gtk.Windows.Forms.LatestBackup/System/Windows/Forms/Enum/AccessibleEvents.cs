// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies events that are reported by accessible applications.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public enum AccessibleEvents
    {
        /// <summary>
        /// <c>EVENT_SYSTEM_SOUND
        /// </c>
        /// Sent when a sound is played. Currently nothing is generating this, we
        /// are going to be cleaning up the SOUNDSENTRY feature in the control panel
        /// and will use this at that time. Applications implementing WinEvents
        /// are perfectly welcome to use it. Clients of IAccessible* will simply
        /// turn around and get back a non-visual object that describes the sound.
        /// </summary>
        SystemSound = 0x0001,

        /// <summary>
        /// An alert was generated. Server applications send this event whenever an important user interface change has occurred that a user might need to know about. The system does not send the event consistently for dialog box objects.
        /// </summary>
        SystemAlert = 0x0002,

        /// <summary>
        /// <c>EVENT_SYSTEM_FOREGROUND
        /// </c>
        /// Sent when the foreground (active) window changes, even if it is changing
        /// to another window in the same thread as the previous one.
        /// </summary>
        SystemForeground = 0x0003,

        /// <summary>
        /// A menu item on the menu bar was selected. The system sends this event for standard menus. Servers send this event for custom menus. The system might raise more than one MenuStart event that might or might not have a corresponding MenuEnd event.
        /// </summary>
        SystemMenuStart = 0x0004,

        /// <summary>
        /// <c>EVENT_SYSTEM_MENUEND
        /// </c>
        /// Sent when leaving from menu mode (system, app bar, and track popups).
        /// </summary>
        /// <seealso cref="SystemMenuStart" />.
        SystemMenuEnd = 0x0005,

        /// <summary>
        /// <c>EVENT_SYSTEM_MENUPOPUPSTART
        /// </c>
        /// Sent when a menu popup comes up.
        /// </summary>
        /// <remarks>
        /// Note that for a call to TrackPopupMenu(), a client will see
        /// <c>EVENT_SYSTEM_MENUSTART
        /// </c>
        /// followed almost immediately by
        /// <c>EVENT_SYSTEM_MENUPOPUPSTART
        /// </c> for the popup being shown.
        /// </remarks>
        /// <seealso cref="SystemMenuPopupEnd" />.
        SystemMenuPopupStart = 0x0006,

        /// <summary>
        /// <c>EVENT_SYSTEM_MENUPOPUPEND
        /// </c>
        /// Sent when a menu popup just before it is taken down.
        /// </summary>
        /// <remarks>
        /// Note that for a call to TrackPopupMenu(), a client will see
        /// <c>EVENT_SYSTEM_MENUSTART
        /// </c>
        /// followed almost immediately by
        /// <c>EVENT_SYSTEM_MENUPOPUPSTART
        /// </c> for the popup being shown.
        /// </remarks>
        /// <seealso cref="SystemMenuPopupStart" />.
        SystemMenuPopupEnd = 0x0007,

        /// <summary>
        /// <c>EVENT_SYSTEM_CAPTURESTART
        /// </c>
        /// Sent when a window takes the capture.
        /// </summary>
        /// <seealso cref="SystemCaptureEnd" />.
        SystemCaptureStart = 0x0008,

        /// <summary>
        /// <c>EVENT_SYSTEM_CAPTUREEND
        /// </c>
        /// Sent when a window releases the capture.
        /// </summary>
        /// <seealso cref="SystemCaptureStart" />.
        SystemCaptureEnd = 0x0009,

        /// <summary>
        /// <c>EVENT_SYSTEM_MOVESIZESTART
        /// </c>
        /// Sent when a window enters move-size dragging mode.
        /// </summary>
        /// <seealso cref="SystemMoveSizeEnd" />.
        SystemMoveSizeStart = 0x000A,

        /// <summary>
        /// <c>EVENT_SYSTEM_MOVESIZEEND
        /// </c>
        /// Sent when a window leaves move-size dragging mode.
        /// </summary>
        /// <seealso cref="SystemMoveSizeStart" />.
        SystemMoveSizeEnd = 0x000B,

        /// <summary>
        /// <c>EVENT_SYSTEM_CONTEXTHELPSTART
        /// </c>
        /// Sent when a window enters context sensitive help mode.
        /// </summary>
        /// <seealso cref="SystemContextHelpEnd" />.
        SystemContextHelpStart = 0x000C,

        /// <summary>
        /// <c>EVENT_SYSTEM_CONTEXTHELPEND
        /// </c>
        /// Sent when a window leaves context sensitive help mode.
        /// </summary>
        /// <seealso cref="SystemContextHelpStart" />.
        SystemContextHelpEnd = 0x000D,

        /// <summary>
        /// <c>EVENT_SYSTEM_DRAGDROPSTART
        /// </c>
        /// Sent when a window enters drag drop mode.
        /// </summary>
        /// <seealso cref="SystemDragDropEnd" />.
        //  Note that it is up to apps and OLE to generate this, since the system doesn't know. 
        //  Like <c>EVENT_SYSTEM_SOUND</c>, it will be a while before this is prevalent.
        SystemDragDropStart = 0x000E,

        /// <summary>
        /// <c>EVENT_SYSTEM_DRAGDROPEND
        /// </c>
        /// Sent when a window leaves drag drop mode.
        /// </summary>
        /// <seealso cref="SystemDragDropStart" />.
        //  Note that it is up to apps and OLE to generate this, since the system doesn't know. 
        //  Like <c>EVENT_SYSTEM_SOUND</c>, it will be a while before this is prevalent.
        SystemDragDropEnd = 0x000F,

        /// <summary>
        /// A dialog box was displayed. The system sends the event for standard dialog boxes. Servers send this event for custom dialog boxes. The system does not send the event consistently.
        /// </summary>
        SystemDialogStart = 0x0010,

        /// <summary>
        /// <c>EVENT_SYSTEM_DIALOGEND
        /// </c>
        /// Sent when a dialog just before it goes away.
        /// </summary>
        /// <seealso cref="SystemDialogStart" />.
        SystemDialogEnd = 0x0011,

        /// <summary>
        /// <c>EVENT_SYSTEM_SCROLLINGSTART
        /// </c>
        /// Sent when beginning the tracking of a scrollbar in a window, and also for scrollbar controls.
        /// </summary>
        /// <seealso cref="SystemScrollingEnd" />.
        SystemScrollingStart = 0x0012,

        /// <summary>
        /// <c>EVENT_SYSTEM_SCROLLINGEND
        /// </c>
        /// Sent when ending the tracking of a scrollbar in a window, and also for scrollbar controls.
        /// </summary>
        /// <seealso cref="SystemScrollingStart" />.
        SystemScrollingEnd = 0x0013,

        /// <summary>
        /// The user pressed ALT+TAB, which activates the switch window. If only one application is running when the user presses ALT+TAB, the system raises the SwitchEnd event without a corresponding SwitchStart event.
        /// </summary>
        SystemSwitchStart = 0x0014,

        /// <summary>
        /// The user released ALT+TAB. The system sends the SwitchEnd event; servers never send this event. If only one application is running when the user presses ALT+TAB, the system sends the SwitchEnd event without a corresponding SwitchStart event.
        /// </summary>
        SystemSwitchEnd = 0x0015,

        /// <summary>
        /// <c>EVENT_SYSTEM_MINIMIZESTART
        /// </c>
        /// Sent when a window minimizes and just before it restores.
        /// </summary>
        /// <seealso cref="SystemMinimizeEnd" />.
        SystemMinimizeStart = 0x0016,

        /// <summary>
        /// <c>EVENT_SYSTEM_MINIMIZEEND
        /// </c>
        /// Sent when a window minimizes and just before it restores.
        /// </summary>
        /// <seealso cref="SystemMinimizeStart" />.
        SystemMinimizeEnd = 0x0017,

        // Object events
        //
        // The system AND apps generate these. The system generates these for
        // real windows. Apps generate these for objects within their window which
        // act like a separate control, e.g. an item in a list view.
        //
        // For all events, if you want detailed accessibility information, callers
        // should
        //      * Call AccessibleObjectFromWindow() with the hwnd, idObject parameters
        //          of the event, and IID_IAccessible as the REFIID, to get back an
        //          IAccessible* to talk to
        //      * Initialize and fill in a VARIANT as VT_I4 with lVal the idChild
        //          parameter of the event.
        //      * If idChild isn't zero, call get_accChild() in the container to see
        //          if the child is an object in its own right. If so, you will get
        //          back an IDispatch* object for the child. You should release the
        //          parent, and call QueryInterface() on the child object to get its
        //          IAccessible*. Then you talk directly to the child. Otherwise,
        //          if get_accChild() returns you nothing, you should continue to
        //          use the child VARIANT. You will ask the container for the properties
        //          of the child identified by the VARIANT. In other words, the
        //          child in this case is accessible but not a full object.
        //          Like a button on a titlebar which is 'small' and has no children.

        /// <summary>
        /// An object was created. The operating system sends the event for the following user interface elements: caret, header control, list view control, tab control, toolbar control, tree view control, and window object. Server applications send this event for their accessible objects. Servers must send this event for all an object's child objects before sending the event for the parent object. Servers must ensure that all child objects are fully created and ready to accept calls from clients when the parent object sends the event.
        /// </summary>
        Create = 0x8000,                // hwnd + ID + idChild is created item

        /// <summary>
        /// An object was destroyed. The system sends this event for the following user interface elements: caret, header control, list view control, tab control, toolbar control, tree view control, and window object. Server applications send this event for their accessible objects. This event may or may not be sent for child objects. However, clients can conclude that all the children of an object have been destroyed when the parent object sends this event.
        /// </summary>
        Destroy = 0x8001,               // hwnd + ID + idChild is destroyed item

        /// <summary>
        /// <c>EVENT_OBJECT_SHOW
        /// </c>
        /// Sent when a hidden object is shown.
        /// The system sends this event for the following user interface elements:
        /// caret, cursor, and window object. Server applications send this event for their accessible objects.
        /// Clients assume that when this event is sent by a parent object, all child objects are already displayed.
        /// Therefore, server applications do not send this event for the child objects.
        /// </summary>
        Show = 0x8002,                  // hwnd + ID + idChild is shown item

        /// <summary>
        /// <c>EVENT_OBJECT_HIDE
        /// </c>
        /// Sent when an object is hidden.
        /// The system sends this event for the following user interface elements: caret and cursor.
        /// Server applications send this event for their accessible objects.
        /// When this event is generated for a parent object, all child objects are already hidden.
        /// Server applications do not send this event for the child objects.
        /// </summary>
        Hide = 0x8003,                  // hwnd + ID + idChild is hidden item

        /// <summary>
        /// <c>EVENT_OBJECT_REORDER
        /// </c>
        /// Sent when a container object has added, removed, or reordered its children.
        /// The system sends this event for the following user interface elements:
        /// header control, list-view control, toolbar control, and window object.
        /// Server applications send this event as appropriate for their accessible objects.
        /// </summary>
        Reorder = 0x8004,               // hwnd + ID + idChild is parent of zordering children

        /// <summary>
        /// Minimize the number of notifications!
        /// When you are hiding a parent object, obviously all child objects are no
        /// longer visible on screen. They still have the same "visible" status,
        /// but are not truly visible. Hence do not send HIDE notifications for the
        /// children also. One implies all. The same goes for SHOW.
        /// </summary>
        Focus = 0x8005,                 // hwnd + ID + idChild is focused item
        /// <summary>
        /// An accessible object within a container object has been selected. This event signals a single selection. Either a child has been selected in a container that previously did not contain any selected children, or the selection has changed from one child to another.
        /// </summary>
        Selection = 0x8006,             // hwnd + ID + idChild is selected item (if only one), or idChild is OBJID_WINDOW if complex
        /// <summary>
        /// An item within a container object was added to the selection. The system sends this event for the following user interface elements: list box, list view control, and tree view control. Server applications send this event for their accessible objects. This event signals that a child has been added to an existing selection.
        /// </summary>
        SelectionAdd = 0x8007,          // hwnd + ID + idChild is item added
        /// <summary>
        /// An item within a container object was removed from the selection. The system sends this event for the following user interface elements: list box, list view control, and tree view control. Server applications send this event for their accessible objects. This event signals that a child has been removed from an existing selection.
        /// </summary>
        SelectionRemove = 0x8008,       // hwnd + ID + idChild is item removed
        /// <summary>
        /// Numerous selection changes occurred within a container object. The system sends this event for list boxes. Server applications send this event for their accessible objects. This event can be sent when the selected items within a control have changed substantially. This event informs the client that many selection changes have occurred. This is preferable to sending several SelectionAdd or SelectionRemove events.
        /// </summary>
        SelectionWithin = 0x8009,       // hwnd + ID + idChild is parent of changed selected items

        /// <summary>
        /// There is only one "focused" child item in a parent. This is the place
        /// keystrokes are going at a given moment. Hence only send a notification
        /// about where the NEW focus is going. A NEW item getting the focus already
        /// implies that the OLD item is losing it.
        ///
        /// SELECTION however can be multiple. Hence the different SELECTION
        /// notifications. Here's when to use each:
        ///
        /// (1) Send a SELECTION notification in the simple single selection
        /// case (like the focus) when the item with the selection is
        /// merely moving to a different item within a container. hwnd + ID
        /// is the container control, idChildItem is the new child with the
        /// selection.
        ///
        /// (2) Send a SELECTIONADD notification when a new item has simply been added
        /// to the selection within a container. This is appropriate when the
        /// number of newly selected items is very small. hwnd + ID is the
        /// container control, idChildItem is the new child added to the selection.
        ///
        /// (3) Send a SELECTIONREMOVE notification when a new item has simply been
        /// removed from the selection within a container. This is appropriate
        /// when the number of newly selected items is very small, just like
        /// SELECTIONADD. hwnd + ID is the container control, idChildItem is the
        /// new child removed from the selection.
        ///
        /// (4) Send a SELECTIONWITHIN notification when the selected items within a
        /// control have changed substantially. Rather than propagate a large
        /// number of changes to reflect removal for some items, addition of
        /// others, just tell somebody who cares that a lot happened. It will
        /// be faster an easier for somebody watching to just turn around and
        /// query the container control what the new bunch of selected items
        /// are.
        /// </summary>
        StateChange = 0x800A,           // hwnd + ID + idChild is item w/ state change
        /// <summary>
        /// An object has changed location, shape, or size. The system sends this event for the following user interface elements: caret and window object. Server applications send this event for their accessible objects. This event is generated in response to the top-level object within the object hierarchy that has changed, not for any children it might contain. For example, if the user resizes a window, the system sends this notification for the window, but not for the menu bar, title bar, scroll bars, or other objects that have also changed. The system does not send this event for every non-floating child window when the parent moves. However, if an application explicitly resizes child windows as a result of being resized, the system sends multiple events for the resized children. If an object's
        /// <see cref="System.Windows.Forms.AccessibleObject.State" /> property is set to
        /// <see cref="System.Windows.Forms.AccessibleStates.Floating" />, servers should send a location change event whenever the object changes location. If an object does not have this state, servers should raise this event when the object moves relative to its parent.
        /// </summary>
        LocationChange = 0x800B,        // hwnd + ID + idChild is moved/sized item
        /// <summary>
        /// An object's
        /// <see cref="System.Windows.Forms.AccessibleObject.Name" /> property changed. The system sends this event for the following user interface elements: check box, cursor, list view control, push button, radio button, status bar control, tree view control, and window object. Server applications send this event for their accessible objects.
        /// </summary>
        NameChange = 0x800C,            // hwnd + ID + idChild is item w/ name change
        /// <summary>
        /// An object's
        /// <see cref="System.Windows.Forms.AccessibleObject.Description" /> property changed. Server applications send this event for their accessible objects.
        /// </summary>
        DescriptionChange = 0x800D,     // hwnd + ID + idChild is item w/ desc change
        /// <summary>
        /// An object's
        /// <see cref="System.Windows.Forms.AccessibleObject.Value" /> property changed. The system raises the ValueChange event for the following user interface elements: edit control, header control, hot key control, progress bar control, scroll bar, slider control, and up-down control. Server applications send this event for their accessible objects.
        /// </summary>
        ValueChange = 0x800E,           // hwnd + ID + idChild is item w/ value change
        /// <summary>
        /// An object has a new parent object. Server applications send this event for their accessible objects.
        /// </summary>
        ParentChange = 0x800F,          // hwnd + ID + idChild is item w/ new parent
        /// <summary>
        /// An object's
        /// <see cref="System.Windows.Forms.AccessibleObject.Help" /> property changed. Server applications send this event for their accessible objects.
        /// </summary>
        HelpChange = 0x8010,            // hwnd + ID + idChild is item w/ help change
        /// <summary>
        /// An object's
        /// <see cref="System.Windows.Forms.AccessibleObject.DefaultAction" /> property changed. The system sends this event for dialog boxes. Server applications send this event for their accessible objects. Therefore, server applications do not need to send this event for the child objects. Hidden objects have a state of
        /// <see cref="System.Windows.Forms.AccessibleStates.Invisible" />, and shown objects do not. Events of type AccessibleEvents.Hide indicate that a state of
        /// <see cref="System.Windows.Forms.AccessibleStates.Invisible" /> has been set. Therefore, servers do not need to send the AccessibleEvents.StateChange event in this case.
        /// </summary>
        DefaultActionChange = 0x8011,   // hwnd + ID + idChild is item w/ def action change
        /// <summary>
        /// An object's
        /// <see cref="System.Windows.Forms.AccessibleObject.KeyboardShortcut" /> property changed. Server applications send the event for their accessible objects.
        /// </summary>
        AcceleratorChange = 0x8012,     // hwnd + ID + idChild is item w/ keybd accel change
    }
}
