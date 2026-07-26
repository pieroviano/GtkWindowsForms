// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Drawing.Printing;

namespace System.Windows.Forms;

/// <summary>Controls how a document is printed, from within a Windows-based application, showing a status
/// dialog. On the GTK layer the status dialog is not shown; the controller delegates to the underlying
/// controller so preview/print still work.</summary>
public class PrintControllerWithStatusDialog : PrintController
{
    private readonly PrintController underlyingController;

    /// <summary>Initializes a new instance wrapping the specified controller.</summary>
    /// <param name="underlyingController">The controller to delegate to.</param>
    public PrintControllerWithStatusDialog(PrintController underlyingController)
        : this(underlyingController, "Printing")
    {
    }

    /// <summary>Initializes a new instance wrapping the specified controller, with a dialog title.</summary>
    /// <param name="underlyingController">The controller to delegate to.</param>
    /// <param name="dialogTitle">The title of the status dialog.</param>
    public PrintControllerWithStatusDialog(PrintController underlyingController, string dialogTitle)
    {
        this.underlyingController = underlyingController;
        DialogTitle = dialogTitle;
    }

    private string DialogTitle { get; }

    /// <summary>Gets a value indicating whether the underlying controller generates a preview.</summary>
    public override bool IsPreview => underlyingController?.IsPreview ?? false;

    /// <inheritdoc />
    public override void OnStartPrint(PrintDocument document, PrintEventArgs e) => underlyingController?.OnStartPrint(document, e);

    /// <inheritdoc />
    public override Graphics? OnStartPage(PrintDocument document, PrintPageEventArgs e) => underlyingController?.OnStartPage(document, e);

    /// <inheritdoc />
    public override void OnEndPage(PrintDocument document, PrintPageEventArgs e) => underlyingController?.OnEndPage(document, e);

    /// <inheritdoc />
    public override void OnEndPrint(PrintDocument document, PrintEventArgs e) => underlyingController?.OnEndPrint(document, e);
}
