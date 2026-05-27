using System;
using System.Windows.Forms;

namespace DateManager
{
    public static class ControlExtensions
    {
        // Invoke가 필요한 경우 안전하게 UI 스레드로 호출해주는 확장 메서드
        public static void InvokeIfRequired(this Control c, Action action)
        {
            if (c == null || c.IsDisposed) return;
            if (c.InvokeRequired)
            {
                try { c.Invoke(action); } catch { }
            }
            else
            {
                try { action(); } catch { }
            }
        }
    }
}
