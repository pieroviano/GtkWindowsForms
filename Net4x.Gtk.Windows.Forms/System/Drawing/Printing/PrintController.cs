// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Drawing.Printing;

/// <summary>Controls how a document is printed, from within a print job.</summary>
public abstract class PrintController
{
    /// <summary>Gets a value indicating whether the controller generates a preview instead of printing.</summary>
    public virtual bool IsPreview => false;

    /// <summary>Called when a print job starts.</summary>
    /// <param name="document">The document being printed.</param>
    /// <param name="e">The event data.</param>
    public virtual void OnStartPrint(PrintDocument document, PrintEventArgs e)
    {
    }

    /// <summary>Called when a page starts; returns the <see cref="T:System.Drawing.Graphics" /> to paint the page.</summary>
    /// <param name="document">The document being printed.</param>
    /// <param name="e">The event data.</param>
    /// <returns>The graphics for the page, or <see langword="null" />.</returns>
    public virtual Graphics? OnStartPage(PrintDocument document, PrintPageEventArgs e) => null;

    /// <summary>Called when a page ends.</summary>
    /// <param name="document">The document being printed.</param>
    /// <param name="e">The event data.</param>
    public virtual void OnEndPage(PrintDocument document, PrintPageEventArgs e)
    {
    }

    /// <summary>Called when a print job ends.</summary>
    /// <param name="document">The document being printed.</param>
    /// <param name="e">The event data.</param>
    public virtual void OnEndPrint(PrintDocument document, PrintEventArgs e)
    {
    }
}
