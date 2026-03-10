// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms;

/// <summary>
/// Provides data for events that need a true or false answer to a question.
/// </summary>
public class QuestionEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionEventArgs"/> class.
    /// </summary>
    public QuestionEventArgs()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QuestionEventArgs"/> class.
    /// </summary>
    /// <param name="response">if set to <c>true</c> [response].</param>
    public QuestionEventArgs(bool response)
    {
        Response = response;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="QuestionEventArgs"/> is response.
    /// </summary>
    /// <value><c>true</c> if response; otherwise, <c>false</c>.</value>
    public bool Response { get; set; }

}
