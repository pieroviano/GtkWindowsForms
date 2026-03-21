// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

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

    }
}