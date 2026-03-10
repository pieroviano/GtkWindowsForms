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
    public ConvertEventArgs(object value, Type desiredType)
    {
        Value = value;
        DesiredType = desiredType;
    }

    public Type DesiredType { get; }

    public object Value { get; set; }

}
