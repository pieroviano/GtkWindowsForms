// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
/// Provides a format-independent mechanism for transferring data.
/// </summary>
[ComVisible(true)]
public interface IDataObject
{
    /// <summary>
    /// Retrieves the data associated with the specified data format, using a Boolean to determine whether to convert the data to the format.
    /// </summary>
    /// <returns>
    /// The data associated with the specified format, or null.
    /// </returns>
    /// <param name="format">
    /// The format of the data to retrieve. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <param name="autoConvert">
    /// true to convert the data to the specified format; otherwise, false.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    object? GetData(string format, bool autoConvert);

    /// <summary>
    /// Retrieves the data associated with the specified data format.
    /// </summary>
    /// <returns>
    /// The data associated with the specified format, or null.
    /// </returns>
    /// <param name="format">
    /// The format of the data to retrieve. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    object? GetData(string format);

    /// <summary>
    /// Retrieves the data associated with the specified class type format.
    /// </summary>
    /// <returns>
    /// The data associated with the specified format, or null.
    /// </returns>
    /// <param name="format">
    /// A
    /// <see cref="T:System.Type" /> representing the format of the data to retrieve. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    object? GetData(Type format);

    /// <summary>
    /// Determines whether data stored in this instance is associated with the specified format, using a Boolean value to determine whether to convert the data to the format.
    /// </summary>
    /// <returns>
    /// true if the data is in, or can be converted to, the specified format; otherwise, false.
    /// </returns>
    /// <param name="format">
    /// The format for which to check. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <param name="autoConvert">
    /// true to determine whether data stored in this instance can be converted to the specified format; false to check whether the data is in the specified format.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    bool GetDataPresent(string format, bool autoConvert);

    /// <summary>
    /// Determines whether data stored in this instance is associated with, or can be converted to, the specified format.
    /// </summary>
    /// <returns>
    /// true if data stored in this instance is associated with, or can be converted to, the specified format; otherwise false.
    /// </returns>
    /// <param name="format">
    /// The format for which to check. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    bool GetDataPresent(string format);

    /// <summary>
    /// Determines whether data stored in this instance is associated with, or can be converted to, the specified format.
    /// </summary>
    /// <returns>
    /// true if data stored in this instance is associated with, or can be converted to, the specified format; otherwise, false.
    /// </returns>
    /// <param name="format">
    /// A
    /// <see cref="T:System.Type" /> representing the format for which to check. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    bool GetDataPresent(Type format);

    /// <summary>
    /// Gets a list of all formats that data stored in this instance is associated with or can be converted to, using a Boolean value to determine whether to retrieve all formats that the data can be converted to or only native data formats.
    /// </summary>
    /// <returns>
    /// An array of the names that represents a list of all formats that are supported by the data stored in this object.
    /// </returns>
    /// <param name="autoConvert">
    /// true to retrieve all formats that data stored in this instance is associated with or can be converted to; false to retrieve only native data formats.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    string[] GetFormats(bool autoConvert);

    /// <summary>
    /// Returns a list of all formats that data stored in this instance is associated with or can be converted to.
    /// </summary>
    /// <returns>
    /// An array of the names that represents a list of all formats that are supported by the data stored in this object.
    /// </returns>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    string[] GetFormats();

    /// <summary>
    /// Stores the specified data and its associated format in this instance, using a Boolean value to specify whether the data can be converted to another format.
    /// </summary>
    /// <param name="format">
    /// The format associated with the data. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <param name="autoConvert">
    /// true to allow the data to be converted to another format; otherwise, false.
    /// </param>
    /// <param name="data">
    /// The data to store.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    void SetData(string format, bool autoConvert, object data);

    /// <summary>
    /// Stores the specified data and its associated format in this instance.
    /// </summary>
    /// <param name="format">
    /// The format associated with the data. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <param name="data">
    /// The data to store.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    void SetData(string format, object data);

    /// <summary>
    /// Stores the specified data and its associated class type in this instance.
    /// </summary>
    /// <param name="format">
    /// A
    /// <see cref="T:System.Type" /> representing the format associated with the data. See
    /// <see cref="!:System.Windows.Forms.DataFormats" /> for predefined formats.
    /// </param>
    /// <param name="data">
    /// The data to store.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    void SetData(Type format, object data);

    /// <summary>
    /// Stores the specified data in this instance, using the class of the data for the format.
    /// </summary>
    /// <param name="data">
    /// The data to store.
    /// </param>
    /// <filterpriority>
    /// 1
    /// </filterpriority>
    void SetData(object data);

}