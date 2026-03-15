// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
/// Provides a designer that can design components that extend Control.
/// </summary>
public partial class ControlDesigner : ComponentDesigner
{
    /// <summary>
    /// Represents a sentinel value indicating an invalid or uninitialized point.
    /// </summary>
    /// <remarks>
    /// This value can be used to signify an error or an undefined state when working with
    /// points. The coordinates are set to the minimum possible integer values, which are unlikely to represent a
    /// valid point in typical scenarios.
    /// </remarks>
#pragma warning disable IDE1006 // Naming Styles - Public API
    protected static readonly Point InvalidPoint = new Point(int.MinValue, int.MinValue);

    private readonly bool _ctrlSelect;                           // if the CTRL key was down at the mouse down

    private readonly CollectionChangeEventHandler _dataBindingsCollectionChanged;

    private Point _downPos = Point.Empty;               // point used to track first down of a double click

    private readonly bool _enabledchangerecursionguard;

    private readonly bool _hadDragDrop;

    //  ...which would cause a cycle.

    private readonly bool _hasLocation;                          // Do we have a location property?

    private readonly IDesignerHost _host;                        // the host for our designer

    private readonly bool _inHitTest;                            // A popular way to implement GetHitTest is by WM_NCHITTEST

    private readonly int _lastClickMessagePositionX;

    private readonly int _lastClickMessagePositionY;

    // Values used to simulate double clicks for controls that don't support them.

    private readonly uint _lastClickMessageTime;

    private readonly int _lastMoveScreenX;

    private readonly int _lastMoveScreenY;

    private readonly bool _liveRegion;                           // is the mouse is over a live region of the control?

    private readonly bool _locationChecked;                      // And did we check it

    private readonly bool _locked;                               // Signifies if this control is locked or not

    // Transient values that are used during mouse drags

    private Point _mouseDragLast = InvalidPoint;        // the last position of the mouse during a drag.

    private readonly bool _mouseDragMoved;                       // has the mouse been moved during this drag?

    private readonly bool _removalNotificationHooked;

    private readonly bool _revokeDragDrop = true;

    private readonly Exception _thrownException;

    private readonly bool _toolPassThrough;                      // a tool is selected, allow the parent to draw a rect for it.

#pragma warning restore IDE1006

    private static readonly uint s_currentProcessId;

    private static readonly bool s_inContextMenu;

    private event EventHandler DisposingHandler;

    internal bool ForceVisible { get; set; } = true;

}