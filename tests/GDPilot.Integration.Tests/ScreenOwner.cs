using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GDPilot.Integration.Tests;

/// <summary>
/// Names the window that owns a screen pixel. Used only to explain a failed
/// capture, where the useful question is what was drawn over the loopback window.
/// </summary>
internal static class ScreenOwner
{
    internal static string Describe(int x, int y)
    {
        NativePoint point = new();
        point.X = x;
        point.Y = y;
        IntPtr window = WindowFromPoint(point);

        if (window == IntPtr.Zero)
        {
            return "no window";
        }

        // The point usually lands on a child; z-order belongs to its top-level window.
        IntPtr root = GetAncestor(window, 2);
        bool topmost = (GetWindowLongPtr(root, -20).ToInt64() & 0x8) != 0;

        StringBuilder title = new(256);
        GetWindowText(window, title, title.Capacity);
        GetWindowThreadProcessId(window, out int processId);

        string process = "unknown";
        try
        {
            using Process owner = Process.GetProcessById(processId);
            process = owner.ProcessName;
        }
        catch (ArgumentException)
        {
            // The owner exited between the lookup and here.
        }

        return $"{process} \"{title}\", top-level {root}, topmost {topmost}";
    }

    internal static bool IsTopmost(IntPtr window)
    {
        return (GetWindowLongPtr(window, -20).ToInt64() & 0x8) != 0;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetAncestor(IntPtr window, uint flags);

    [DllImport("user32.dll")]
    private static extern IntPtr GetWindowLongPtr(IntPtr window, int index);

    [StructLayout(LayoutKind.Sequential)]
    private struct NativePoint
    {
        internal int X;
        internal int Y;
    }

    [DllImport("user32.dll")]
    private static extern IntPtr WindowFromPoint(NativePoint point);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern int GetWindowText(IntPtr window, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr window, out int processId);
}
