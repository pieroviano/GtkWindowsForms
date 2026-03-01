using System;
using System.Collections.Generic;
using System.Text;

namespace System.Windows.Forms
{
    public static class FormExtension
    {
        public static async Task CloseAfterDelayAsync(this Form form)
        {
            await Task.Delay(100);
            form.Close();
        }
    }
}
