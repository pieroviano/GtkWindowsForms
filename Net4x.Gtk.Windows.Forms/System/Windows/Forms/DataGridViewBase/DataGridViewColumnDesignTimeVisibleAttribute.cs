// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
/// Specifies whether a data-grid-view column type is visible in the designer's column collection editor.
/// It was absent, so a custom column type annotated with it did not compile.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class DataGridViewColumnDesignTimeVisibleAttribute : Attribute
{
    /// <summary>Represents a column type that is visible at design time.</summary>
    public static readonly DataGridViewColumnDesignTimeVisibleAttribute Yes = new(true);

    /// <summary>Represents a column type that is not visible at design time.</summary>
    public static readonly DataGridViewColumnDesignTimeVisibleAttribute No = new(false);

    /// <summary>Represents the default value, <see cref="Yes" />.</summary>
    public static readonly DataGridViewColumnDesignTimeVisibleAttribute Default = Yes;

    /// <summary>Initializes a new instance that is visible at design time.</summary>
    public DataGridViewColumnDesignTimeVisibleAttribute() : this(true)
    {
    }

    /// <summary>Initializes a new instance with the given visibility.</summary>
    /// <param name="visible">Whether the column type is visible at design time.</param>
    public DataGridViewColumnDesignTimeVisibleAttribute(bool visible)
    {
        Visible = visible;
    }

    /// <summary>Gets a value indicating whether the column type is visible at design time.</summary>
    public bool Visible { get; }

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is DataGridViewColumnDesignTimeVisibleAttribute other && other.Visible == Visible;

    /// <inheritdoc />
    public override int GetHashCode() => Visible.GetHashCode();

    /// <inheritdoc />
    public override bool IsDefaultAttribute() => Visible == Default.Visible;
}
