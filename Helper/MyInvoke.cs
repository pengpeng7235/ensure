using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Measure.Helper
{
    public static class MyInvoke
    {
        public static void DoOnUiThread(this Control control, Action action)
        {
            if (control.IsDisposed) return;

            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();
        }
    }
}
