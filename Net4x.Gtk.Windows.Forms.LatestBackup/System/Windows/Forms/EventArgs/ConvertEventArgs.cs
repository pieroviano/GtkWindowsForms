// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the
    /// <see cref="System.Windows.Forms.Binding.Format" /> and
    /// <see cref="System.Windows.Forms.Binding.Parse" /> events.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class ConvertEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="System.Windows.Forms.ConvertEventArgs" /> class.
        /// </summary>
        /// <param name="value">An
        /// <see cref="System.Object" /> that contains the value of the current property.
        /// </param>
        /// <param name="desiredType">The
        /// <see cref="System.Type" /> of the value.
        /// </param>
        public ConvertEventArgs(object value, Type desiredType)
        {
            Value = value;
            DesiredType = desiredType;
        }

        /// <summary>
        /// Gets the data type of the desired value.
        /// </summary>
        /// <returns>The
        /// <see cref="System.Type" /> of the desired value.
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public Type DesiredType { get; }

        /// <summary>
        /// Gets or sets the value of the
        /// <see cref="System.Windows.Forms.ConvertEventArgs" />
        /// </summary>
        /// <returns>The value of the
        /// <see cref="System.Windows.Forms.ConvertEventArgs" />
        /// </returns>
        /// <filterpriority>1
        /// </filterpriority>
        public object Value { get; set; }

    }
}
