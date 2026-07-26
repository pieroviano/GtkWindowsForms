// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Provides information about how acceleration should be performed on a spin box (up-down control) when the up or down button is pressed for a length of time.</summary>
public class NumericUpDownAcceleration
{
    private int seconds;
    private decimal increment;

    /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.NumericUpDownAcceleration" /> class.</summary>
    /// <param name="seconds">The number of seconds the up or down button is pressed before the acceleration begins.</param>
    /// <param name="increment">The quantity to increment or decrement the value by during acceleration.</param>
    public NumericUpDownAcceleration(int seconds, decimal increment)
    {
        if (seconds < 0) { throw new ArgumentOutOfRangeException(nameof(seconds)); }
        if (increment < 0m) { throw new ArgumentOutOfRangeException(nameof(increment)); }
        this.seconds = seconds;
        this.increment = increment;
    }

    /// <summary>Gets or sets the number of seconds the button is pressed before acceleration starts.</summary>
    public int Seconds
    {
        get => seconds;
        set { if (value < 0) { throw new ArgumentOutOfRangeException(nameof(value)); } seconds = value; }
    }

    /// <summary>Gets or sets the increment applied during acceleration.</summary>
    public decimal Increment
    {
        get => increment;
        set { if (value < 0m) { throw new ArgumentOutOfRangeException(nameof(value)); } increment = value; }
    }
}
