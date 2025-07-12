using System.Runtime.InteropServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppControl.Controls
{
    public static class ScrollControls
    {

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
            public nint dwExtraInfo;
        }

        [DllImport("user32.dll")]
        static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        const uint INPUT_MOUSE = 0;
        const uint MOUSEEVENTF_HWHEEL = 0x01000;  // cờ cuộn ngang
        const uint MOUSEEVENTF_WHEEL = 0x0800; // cờ cuộn xuống

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
