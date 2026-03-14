// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
/// PaperSource specifies the source of paper to use when printing. It is used by the
/// <see cref="T:System.Drawing.Printing.PageSettings" /> class to specify the source of paper to use when printing a document.
/// </summary>
public partial class PaperSource
{
    private PaperSourceKind _kind;

    private string _name;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Printing.PaperSource" /> class.
    /// </summary>
    public PaperSource()
    {
        _kind = PaperSourceKind.Custom;
        _name = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Drawing.Printing.PaperSource" /> class.
    /// </summary>
    internal PaperSource(PaperSourceKind kind, string name)
    {
        _kind = kind;
        _name = name;
    }

    /// <summary>
    /// Gets the kind.
    /// </summary>
    /// <value>
    /// The kind.
    /// </value>
    public PaperSourceKind Kind => _kind;

    /// <summary>
    /// Gets or sets the kind of the raw.
    /// </summary>
    /// <value>
    /// The kind of the raw.
    /// </value>
    public int RawKind
    {
        get => (int)_kind;
        set => _kind = (PaperSourceKind)value;
    }

    /// <summary>
    /// Gets or sets the name of the source.
    /// </summary>
    /// <value>
    /// The name of the source.
    /// </value>
    public string SourceName
    {
        get => _name;
        set => _name = value;
    }

    /// <summary>
    /// Converts to string.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString() => $"[PaperSource {SourceName} Kind={Kind}]";

}