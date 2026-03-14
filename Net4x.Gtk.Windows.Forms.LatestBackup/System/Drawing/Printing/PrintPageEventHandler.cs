// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
/// Represents the method that will handle the
/// <see cref="T:System.Drawing.Printing.PrintDocument.PrintPage" /> event of a
/// <see cref="T:System.Drawing.Printing.PrintDocument" />.
/// </summary>
public delegate void PrintPageEventHandler(object sender, PrintPageEventArgs e);