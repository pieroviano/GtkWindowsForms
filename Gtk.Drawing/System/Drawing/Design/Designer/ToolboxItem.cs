// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Runtime.Serialization;
using System.ComponentModel.Design;
using System.ComponentModel;

namespace System.Drawing.Design;

/// <summary>
/// Provides a base implementation of a toolbox item.
/// </summary>
public class ToolboxItem : ISerializable
{
    private const int ICON_DIMENSION = 16;

    private static readonly int s_iconHeight = ICON_DIMENSION;

    private static readonly int s_iconWidth = ICON_DIMENSION;

    private static readonly bool s_isScalingInitialized;

    private readonly Type? _toolType;

    /// <summary>
    /// Initializes a new instance of the ToolboxItem class.
    /// </summary>
    public ToolboxItem()
    {
    }

    /// <summary>
    /// Initializes a new instance of the ToolboxItem class using the specified type.
    /// </summary>
    public ToolboxItem(Type? toolType) : this()
    {
        _toolType = toolType;
    }

    /// <summary>
    /// Gets the Type represented by this ToolboxItem, if any.
    /// </summary>
    public Type? ToolType => _toolType;

    /// <summary>
    /// Create components represented by this ToolboxItem using a designer host.
    /// This provides a minimal, test-friendly implementation so callers can
    /// create instances when a real framework ToolboxItem.CreateComponents is not available.
    /// </summary>
    /// <param name="host">The designer host used to create components.</param>
    /// <returns>An array of created components (may be empty).</returns>
    public virtual object[] CreateComponents(IDesignerHost? host)
    {
        if (_toolType == null || host == null)
        {
            return Array.Empty<object>();
        }

        var component = host.CreateComponent(_toolType);
        return component is null ? Array.Empty<object>() : new object[] { component };
    }

    /// <summary>
    /// Populates a
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo">
    /// </see> with the data needed to serialize the target object.
    /// </summary>
    /// <param name="info">
    /// The
    /// <see cref="T:System.Runtime.Serialization.SerializationInfo">
    /// </see> to populate with data.
    /// </param>
    /// <param name="context">
    /// The destination (see
    /// <see cref="T:System.Runtime.Serialization.StreamingContext">
    /// </see>) for this serialization.
    /// </param>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        // Minimal shim: nothing to serialize for tests.
    }
}