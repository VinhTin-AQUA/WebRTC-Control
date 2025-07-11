using System.Runtime.InteropServices;

namespace AppControl
{
    public static class ActionControls
    {
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        public static void LeftMouseClick(int x, int y)
        {
            SetCursorPos(x, y); // Di chuyển chuột đến tọa độ
            Thread.Sleep(100); // Delay để đảm bảo chuột đã đến đúng vị trí
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }

        public static void RightMouseClick(int x, int y)
        {
            SetCursorPos(x, y); // Di chuyển chuột đến tọa độ
            Thread.Sleep(100); // Delay để đảm bảo chuột đã đến đúng vị trí
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
        }
    }
}
