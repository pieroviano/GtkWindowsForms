// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;
/// <summary>
/// Contains border styles for the cells in a
/// <see cref="T:System.Windows.Forms.DataGridView">
/// DataGridView
/// </see> control.
/// </summary>
/// <filterpriority>
/// 2
/// </filterpriority>
public sealed class DataGridViewAdvancedBorderStyle : ICloneable
{

    private bool _all = true;

    private readonly DataGridViewAdvancedCellBorderStyle _banned1;

    private readonly DataGridViewAdvancedCellBorderStyle _banned2;

    private readonly DataGridViewAdvancedCellBorderStyle _banned3;

    private DataGridViewAdvancedCellBorderStyle _bottom = DataGridViewAdvancedCellBorderStyle.None;

    private DataGridViewAdvancedCellBorderStyle _left = DataGridViewAdvancedCellBorderStyle.None;

    private readonly DataGridView? _owner;

    private DataGridViewAdvancedCellBorderStyle _right = DataGridViewAdvancedCellBorderStyle.None;

    private DataGridViewAdvancedCellBorderStyle _top = DataGridViewAdvancedCellBorderStyle.None;

    /// <summary>
    /// Initializes a new instance of the
    /// <see cref="T:System.Windows.Forms.DataGridViewAdvancedBorderStyle" /> class.
    /// </summary>
    public DataGridViewAdvancedBorderStyle()
            : this(
                null,
                DataGridViewAdvancedCellBorderStyle.NotSet,
                DataGridViewAdvancedCellBorderStyle.NotSet,
                DataGridViewAdvancedCellBorderStyle.NotSet)
    {
    }

    internal DataGridViewAdvancedBorderStyle(DataGridView owner)
        : this(
            owner,
            DataGridViewAdvancedCellBorderStyle.NotSet,
            DataGridViewAdvancedCellBorderStyle.NotSet,
            DataGridViewAdvancedCellBorderStyle.NotSet)
    {
    }

    /// <summary>
    /// Creates a new DataGridViewAdvancedBorderStyle. The specified owner will
    /// be notified when the values are changed.
    /// </summary>
    internal DataGridViewAdvancedBorderStyle(
        DataGridView? owner,
        DataGridViewAdvancedCellBorderStyle banned1,
        DataGridViewAdvancedCellBorderStyle banned2,
        DataGridViewAdvancedCellBorderStyle banned3)
    {
        _owner = owner;
        _banned1 = banned1;
        _banned2 = banned2;
        _banned3 = banned3;
    }

    /// <summary>
    /// Gets or sets all.
    /// </summary>
    /// <value>
    /// All.
    /// </value>
    public DataGridViewAdvancedCellBorderStyle All
    {
        get
        {
            return _all ? _top : DataGridViewAdvancedCellBorderStyle.NotSet;
        }

        set
        {

        }
    }

    /// <summary>
    /// Gets or sets the bottom.
    /// </summary>
    /// <value>
    /// The bottom.
    /// </value>
    public DataGridViewAdvancedCellBorderStyle Bottom
    {
        get
        {
            if (_all)
            {
                return _top;
            }

            return _bottom;
        }

        set
        {

        }
    }

    /// <summary>
    /// Gets or sets the left.
    /// </summary>
    /// <value>
    /// The left.
    /// </value>
    public DataGridViewAdvancedCellBorderStyle Left
    {
        get
        {
            if (_all)
            {
                return _top;
            }

            return _left;
        }

        set
        {

        }
    }

    /// <summary>
    /// Gets or sets the right.
    /// </summary>
    /// <value>
    /// The right.
    /// </value>
    public DataGridViewAdvancedCellBorderStyle Right
    {
        get
        {
            if (_all)
            {
                return _top;
            }

            return _right;
        }

        set
        {

        }
    }

    /// <summary>
    /// Gets or sets the top.
    /// </summary>
    /// <value>
    /// The top.
    /// </value>
    public DataGridViewAdvancedCellBorderStyle Top
    {
        get
        {
            return _top;
        }

        set
        {

        }
    }

    internal DataGridViewAdvancedCellBorderStyle BottomInternal
    {
        set
        {

        }
    }

    internal DataGridViewAdvancedCellBorderStyle LeftInternal
    {
        set
        {

        }
    }

    internal DataGridViewAdvancedCellBorderStyle RightInternal
    {
        set
        {

        }
    }

    internal DataGridViewAdvancedCellBorderStyle TopInternal
    {
        set
        {

        }
    }

    /// <summary>
    /// Determines whether the specified
    /// <see cref="T:System.Object" /> is equal to this instance.
    /// </summary>
    /// <param name="other">
    /// The
    /// <see cref="T:System.Object" /> to compare with this instance.
    /// </param>
    /// <returns>
    /// <c>
    /// true
    /// </c> if the specified
    /// <see cref="T:System.Object" /> is equal to this instance; otherwise,
    /// <c>
    /// false
    /// </c>.
    /// </returns>
    public override bool Equals(object? other)
    {
        if (other is not DataGridViewAdvancedBorderStyle dgvabsOther)
        {
            return false;
        }

        return dgvabsOther._all == _all &&
            dgvabsOther._top == _top &&
            dgvabsOther._left == _left &&
            dgvabsOther._bottom == _bottom &&
            dgvabsOther._right == _right;
    }

    /// <summary>
    /// Returns a hash code for this instance.
    /// </summary>
    /// <returns>
    /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
    /// </returns>
    public override int GetHashCode() => HashCode.Combine(_top, _left, _bottom, _right);

    /// <summary>
    /// Returns a
    /// <see cref="T:System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A
    /// <see cref="T:System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"DataGridViewAdvancedBorderStyle {{ All={All}, Left={Left}, Right={Right}, Top={Top}, Bottom={Bottom} }}";
    }

    object ICloneable.Clone()
    {
        DataGridViewAdvancedBorderStyle dgvabs = new(_owner, _banned1, _banned2, _banned3)
        {
            _all = _all,
            _top = _top,
            _right = _right,
            _bottom = _bottom,
            _left = _left
        };

        return dgvabs;
    }

}