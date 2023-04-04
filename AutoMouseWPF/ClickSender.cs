using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace AutoMouseWPF
{
    public class ClickSender
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int x;
            public int y;
        }

        [Flags]
        public enum MouseEventFlags : uint
        {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010,
            Wheel = 0x00000800,
            XDown = 0x00000080,
            XUp = 0x00000100
        }

        public static void SendClick(MouseButton button, Point? point = null)
        {
            if (!point.HasValue)
            {
                if (!GetCursorPos(out POINT p))
                {
                    throw new InvalidOperationException("Failed to get cursor position");
                }
                point = new Point(p.x, p.y);
            }

            uint x = (uint)(65535 * point.Value.X / SystemParameters.PrimaryScreenWidth);
            uint y = (uint)(65535 * point.Value.Y / SystemParameters.PrimaryScreenHeight);
            uint data = 0;
            UIntPtr extraInfo = UIntPtr.Zero;

            switch (button)
            {
                case MouseButton.Left:
                    mouse_event((uint)(MouseEventFlags.LeftDown | MouseEventFlags.LeftUp), x, y, data, extraInfo);
                    break;
                case MouseButton.Middle:
                    mouse_event((uint)(MouseEventFlags.MiddleDown | MouseEventFlags.MiddleUp), x, y, data, extraInfo);
                    break;
                case MouseButton.Right:
                    mouse_event((uint)(MouseEventFlags.RightDown | MouseEventFlags.RightUp), x, y, data, extraInfo);
                    break;
                default:
                    throw new ArgumentException("Invalid mouse button");
            }
        }
    }
}
