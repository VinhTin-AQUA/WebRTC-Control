using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppControl
{
    public static class ActionControls
    {
        #region mouse event

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;

        #endregion

        #region mouse scroll event

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll")]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        const uint INPUT_MOUSE = 0;
        const uint MOUSEEVENTF_HWHEEL = 0x01000;  // cờ cuộn ngang
        const uint MOUSEEVENTF_WHEEL = 0x0800; // cờ cuộn xuống
        

        #endregion

        public static void SetMousePos(int mouseX, int mouseY)
        {
            SetCursorPos(mouseX, mouseY);
        }

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

        // cuộn ngang
        public static void ScrollHorizontal(int amount)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].mi.dwFlags = MOUSEEVENTF_HWHEEL;
            inputs[0].mi.mouseData = (uint)amount;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        // cuộn xuống
        public static void ScrollVerticle(int amount)
        {
            INPUT[] inputs = new INPUT[1];
            inputs[0].type = INPUT_MOUSE;
            inputs[0].mi.dwFlags = MOUSEEVENTF_WHEEL;
            inputs[0].mi.mouseData = (uint)amount;
            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
