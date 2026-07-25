// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>Defines a method that executes a command.</summary>
public interface ICommandExecutor
{
    /// <summary>Executes the command.</summary>
    void Execute();
}
