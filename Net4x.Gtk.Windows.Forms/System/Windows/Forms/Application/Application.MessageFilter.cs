// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Windows.Forms;

/// <summary>
/// Message-filter registration. Mirrors <c>System.Windows.Forms.Application</c> so code that installs an
/// <see cref="T:System.Windows.Forms.IMessageFilter" /> compiles.
/// </summary>
/// <remarks>
/// GTK does not surface a Win32 message pump, so registered filters are not invoked by an internal pump;
/// they are stored and can be run explicitly via
/// <see cref="M:System.Windows.Forms.Application.FilterMessage(System.Windows.Forms.Message@)" />. This is a
/// documented boundary of the GTK layer.
/// </remarks>
public sealed partial class Application
{
    private static readonly List<IMessageFilter> _messageFilters = new();

    /// <summary>Adds a message filter to monitor Windows messages as they are routed to their destinations.</summary>
    /// <param name="value">The <see cref="T:System.Windows.Forms.IMessageFilter" /> to add.</param>
    public static void AddMessageFilter(IMessageFilter value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        lock (_messageFilters)
        {
            _messageFilters.Add(value);
        }
    }

    /// <summary>Removes a message filter from the message pump of the application.</summary>
    /// <param name="value">The <see cref="T:System.Windows.Forms.IMessageFilter" /> to remove.</param>
    public static void RemoveMessageFilter(IMessageFilter value)
    {
        lock (_messageFilters)
        {
            _messageFilters.Remove(value);
        }
    }

    /// <summary>Runs any message filters against <paramref name="message" />.</summary>
    /// <param name="message">The message to filter, passed by reference.</param>
    /// <returns><see langword="true" /> if a filter consumed the message; otherwise <see langword="false" />.</returns>
    public static bool FilterMessage(ref Message message)
    {
        IMessageFilter[] snapshot;
        lock (_messageFilters)
        {
            snapshot = _messageFilters.ToArray();
        }

        foreach (var filter in snapshot)
        {
            if (filter.PreFilterMessage(ref message))
            {
                return true;
            }
        }

        return false;
    }
}
