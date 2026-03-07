// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;
/// <summary>
/// Contains border styles for the cells in a
/// <see cref="System.Windows.Forms.DataGridView" /> control.
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
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" /> class.
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
    /// Gets or sets the border style for all of the borders of a cell.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" /> values.
    /// </returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" /> values.
    /// </exception>
    /// <exception cref="System.ArgumentException">The specified value when setting this property is
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.NotSet" />-or-The specified value when setting this property is
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.OutsetDouble" />,
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.OutsetPartial" />, or
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.InsetDouble" /> and this
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" /> instance was retrieved through the
    /// <see cref="System.Windows.Forms.DataGridView.AdvancedCellBorderStyle" /> property.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence" />
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true" />
    /// </PermissionSet>
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
    /// Gets or sets the style for the bottom border of a cell.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" /> values.
    /// </returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" /> values.
    /// </exception>
    /// <exception cref="System.ArgumentException">The specified value when setting this property is
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.NotSet" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
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
    /// Gets the style for the left border of a cell.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" /> values.
    /// </returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" />
    /// </exception>
    /// <exception cref="System.ArgumentException">The specified value when setting this property is
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.NotSet" />-or-The specified value when setting this property is
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.InsetDouble" /> or
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.OutsetDouble" /> and this
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" /> instance has an associated
    /// <see cref="System.Windows.Forms.DataGridView" /> control with a
    /// <see cref="System.Windows.Forms.Control.RightToLeft" /> property value of true.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
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
    /// Gets the style for the right border of a cell.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" /> values.
    /// </returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" />
    /// </exception>
    /// <exception cref="System.ArgumentException">The specified value when setting this property is
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.NotSet" />-or-The specified value when setting this property is
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.InsetDouble" /> or
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.OutsetDouble" /> and this
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" /> instance has an associated
    /// <see cref="System.Windows.Forms.DataGridView" /> control with a
    /// <see cref="System.Windows.Forms.Control.RightToLeft" /> property value of false.
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
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
    /// Gets the style for the top border of a cell.
    /// </summary>
    /// <returns>One of the
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" /> values.
    /// </returns>
    /// <exception cref="System.ComponentModel.InvalidEnumArgumentException">The specified value when setting this property is not a valid
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle" />
    /// </exception>
    /// <exception cref="System.ArgumentException">The specified value when setting this property is
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.NotSet" />
    /// </exception>
    /// <filterpriority>1
    /// </filterpriority>
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
    /// Determines whether the specified object is equal to the current
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" />
    /// </summary>
    /// <returns>true if
    /// <paramref name="other" /> is a
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" /> and the values for the
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle.Top" />,
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle.Bottom" />,
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle.Left" />, and
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle.Right" /> properties are equal to their counterpart in the current
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" />; otherwise, false.
    /// </returns>
    /// <param name="other">An
    /// <see cref="System.Object" /> to be compared.
    /// </param>
    /// <filterpriority>1
    /// </filterpriority>
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

    /// <filterpriority>
    /// 1
    /// </filterpriority>
    public override int GetHashCode() => HashCode.Combine(_top, _left, _bottom, _right);

    /// <summary>
    /// Returns a string that represents the
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" />
    /// </summary>
    /// <returns>A string that represents the
    /// <see cref="System.Windows.Forms.DataGridViewAdvancedBorderStyle" />
    /// </returns>
    /// <filterpriority>1
    /// </filterpriority>
    public override string ToString()
    {
        return $"DataGridViewAdvancedBorderStyle {{ All={All}, Left={Left}, Right={Right}, Top={Top}, Bottom={Bottom} }}";
    }

    /// <summary>
    /// Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns>A copy of the current instance.
    /// </returns>
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
