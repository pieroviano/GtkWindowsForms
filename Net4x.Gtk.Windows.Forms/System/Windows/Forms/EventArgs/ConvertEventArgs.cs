// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.Binding.Format" /> and
/// <see cref="T:System.Windows.Forms.Binding.Parse" /> events.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public class ConvertEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.ConvertEventArgs" /> class.
    /// </summary>
    /// <param name="value">
    /// The value.
    /// </param>
    /// <param name="desiredType">
    /// Type of the desired.
    /// </param>
    public ConvertEventArgs(object value, Type desiredType)
    {
        Value = value;
        DesiredType = desiredType;
    }

    /// <summary>
    /// Gets the type of the desired.
    /// </summary>
    /// <value>
    /// The type of the desired.
    /// </value>
    public Type DesiredType { get; }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    /// <value>
    /// The value.
    /// </value>
    public object Value
    {
        get;
        set;
    }

}