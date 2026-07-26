// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Drawing.Printing;

/// <summary>Specifies a print controller that displays a document on a screen as a series of images.</summary>
public class PreviewPrintController : PrintController
{
    private readonly List<PreviewPageInfo> pages = new();
    private Bitmap? current;

    /// <summary>Gets a value indicating that this controller is a preview controller.</summary>
    public override bool IsPreview => true;

    /// <summary>Gets or sets a value indicating whether anti-aliasing is used when displaying the preview.</summary>
    public virtual bool UseAntiAlias { get; set; }

    /// <inheritdoc />
    public override void OnStartPrint(PrintDocument document, PrintEventArgs e)
    {
        pages.Clear();
        current = null;
    }

    /// <inheritdoc />
    public override Graphics? OnStartPage(PrintDocument document, PrintPageEventArgs e)
    {
        var bounds = e?.PageBounds ?? Rectangle.Empty;
        var w = bounds.Width > 0 ? bounds.Width : 850;
        var h = bounds.Height > 0 ? bounds.Height : 1100;
        current = new Bitmap(w, h);
        return Graphics.FromImage(current);
    }

    /// <inheritdoc />
    public override void OnEndPage(PrintDocument document, PrintPageEventArgs e)
    {
        if (current != null)
        {
            pages.Add(new PreviewPageInfo(current, new Size(current.Width, current.Height)));
            current = null;
        }
    }

    /// <summary>Captures the pages of a document as a series of images.</summary>
    /// <returns>An array of <see cref="T:System.Drawing.Printing.PreviewPageInfo" /> objects, one per page.</returns>
    public PreviewPageInfo[] GetPreviewPageInfo() => pages.ToArray();
}
