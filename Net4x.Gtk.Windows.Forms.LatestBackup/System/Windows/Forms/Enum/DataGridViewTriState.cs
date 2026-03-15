using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms;


/// <summary>
/// Defines values for specifying how the width of a column is adjusted.
/// </summary>
public enum DataGridViewTriState
{

    /// <summary>
    /// The property is not set and will behave in a default manner.
    /// </summary>
    NotSet = 0,

    /// <summary>
    /// The property's state is true.
    /// </summary>
    True = 1,

    /// <summary>
    /// The property's state is false.
    /// </summary>
    False = 2
}