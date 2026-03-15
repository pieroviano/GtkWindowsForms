// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Threading;

namespace System.Runtime.Remoting.Messaging;
//namespace System

/// <summary>
/// AsyncResult is the default implementation of the IAsyncResult interface. It is used by the remoting infrastructure to provide a default implementation of the IAsyncResult interface for asynchronous method calls.
/// </summary>
/// <seealso cref="T:System.IAsyncResult" />
public class AsyncResult : IAsyncResult
{
    /// <summary>
    /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
    /// </summary>
    public object AsyncState { get; set; }

    /// <summary>
    /// Gets a
    /// <see cref="T:System.Threading.WaitHandle">
    /// </see> that is used to wait for an asynchronous operation to complete.
    /// </summary>
    public WaitHandle AsyncWaitHandle => new EventWaitHandle(false, EventResetMode.AutoReset);

    /// <summary>
    /// Gets a value that indicates whether the asynchronous operation completed synchronously.
    /// </summary>
    public bool CompletedSynchronously { get; set; }

    /// <summary>
    /// Gets a value that indicates whether the asynchronous operation has completed.
    /// </summary>
    public bool IsCompleted { get; set; }

}