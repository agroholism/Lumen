using System;
using System.Windows.Forms;

namespace Lumen.Studio {
    public static class AsyncHelper {
        public static void InvokeNeded(this Control control, Action action) {
            try {
                if (control.InvokeRequired) {
                    control.Invoke(action);
                } else {
                    action();
                }
            } catch { }
        }
    }
}
