// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

namespace System.Drawing.Printing;

/// <summary>Represents the exception that is thrown when trying to access a printer using invalid printer settings.</summary>
[Serializable]
public class InvalidPrinterException : SystemException
{
    /// <summary>Initializes a new instance of the <see cref="T:System.Drawing.Printing.InvalidPrinterException" /> class.</summary>
    /// <param name="settings">The printer settings that are not valid.</param>
    public InvalidPrinterException(PrinterSettings settings)
        : base("Settings to access printer '" + (settings?.PrinterName ?? string.Empty) + "' are not valid.")
    {
    }

    /// <summary>Initializes a new instance from serialized data.</summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    protected InvalidPrinterException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the exception.</summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The streaming context.</param>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
        => base.GetObjectData(info, context);
}
