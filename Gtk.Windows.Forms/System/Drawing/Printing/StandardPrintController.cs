// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing.Printing;

/// <summary>Specifies a print controller that sends information to a printer.</summary>
public class StandardPrintController : PrintController
{
    /// <inheritdoc />
    public override void OnStartPrint(PrintDocument document, PrintEventArgs e) => base.OnStartPrint(document, e);

    /// <inheritdoc />
    public override Graphics? OnStartPage(PrintDocument document, PrintPageEventArgs e) => e?.Graphics;

    /// <inheritdoc />
    public override void OnEndPage(PrintDocument document, PrintPageEventArgs e) => base.OnEndPage(document, e);

    /// <inheritdoc />
    public override void OnEndPrint(PrintDocument document, PrintEventArgs e) => base.OnEndPrint(document, e);
}
