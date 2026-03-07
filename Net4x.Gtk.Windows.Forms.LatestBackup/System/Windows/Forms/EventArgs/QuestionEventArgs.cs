// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for events that need a true or false answer to a question.
    /// </summary>
    /// <filterpriority>
    /// 2
    /// </filterpriority>
    public class QuestionEventArgs : EventArgs
    {
        public QuestionEventArgs()
        {
        }

        public QuestionEventArgs(bool response)
        {
            Response = response;
        }

        public bool Response { get; set; }
    }
}
