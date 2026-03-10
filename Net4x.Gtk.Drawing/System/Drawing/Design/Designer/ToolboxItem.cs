// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Drawing.Design
{
    /// <summary>
    /// Provides a base implementation of a toolbox item.
    /// </summary>
    public class ToolboxItem : ISerializable
    {

        private const int ICON_DIMENSION = 16;

        private static int s_iconHeight = ICON_DIMENSION;

        private static int s_iconWidth = ICON_DIMENSION;

        private static bool s_isScalingInitialized;

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Drawing.Design.ToolboxItem" /> class.
        /// </summary>
        public ToolboxItem()
        {

        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Drawing.Design.ToolboxItem" /> class that creates the specified type of component.
        /// </summary>
        /// <param name="toolType">
        /// The type of
        /// <see cref="T:System.ComponentModel.IComponent" /> that the toolbox item creates.
        /// </param>
        /// <exception cref="T:System.InvalidOperationException">
        /// The
        /// <see cref="T:System.Drawing.Design.ToolboxItem" /> was locked.
        /// </exception>
        public ToolboxItem(Type? toolType) : this()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

    }
}
