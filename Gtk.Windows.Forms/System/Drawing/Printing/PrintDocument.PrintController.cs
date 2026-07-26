// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing.Printing;

/// <summary>Exposes the print controller that governs a print job. Separated from the main
/// <see cref="T:System.Drawing.Printing.PrintDocument" /> definition.</summary>
public partial class PrintDocument
{
    private PrintController? _printController;

    /// <summary>Gets or sets the print controller that guides the printing process.</summary>
    public PrintController PrintController
    {
        get => _printController ??= new StandardPrintController();
        set => _printController = value;
    }
}
