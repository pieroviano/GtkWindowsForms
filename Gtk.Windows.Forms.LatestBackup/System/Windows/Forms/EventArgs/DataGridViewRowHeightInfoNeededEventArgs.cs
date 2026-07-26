// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for the
/// <see cref="T:System.Windows.Forms.DataGridView.RowHeightInfoNeeded" /> event of a
/// <see cref="T:System.Windows.Forms.DataGridView" />
/// </summary>
public class DataGridViewRowHeightInfoNeededEventArgs : EventArgs
{

    private readonly int _height;

    private readonly int _minimumHeight;

    internal DataGridViewRowHeightInfoNeededEventArgs()
    {
        RowIndex = -1;
        _height = -1;
        _minimumHeight = -1;
    }

    /// <summary>
    /// Gets the index of the row.
    /// </summary>
    /// <value>
    /// The index of the row.
    /// </value>
    public int RowIndex
    {
        get;
        private set;
    }

    //        _height = value;
    //    }
    //}

    //        if (_height < value)
    //        {
    //            _height = value;
    //        }
    //        _minimumHeight = value;
    //    }
    //}

    //internal void SetProperties(int rowIndex, int height, int minimumHeight)
    //{
    //    Debug.Assert(rowIndex >= -1);
    //    Debug.Assert(height > 0);
    //    Debug.Assert(minimumHeight > 0);
    //    Debug.Assert(height >= minimumHeight);
    //    RowIndex = rowIndex;
    //    _height = height;
    //    _minimumHeight = minimumHeight;
    //}
}