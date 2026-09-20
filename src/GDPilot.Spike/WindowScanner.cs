using System.Diagnostics;
using System.Text;

namespace GDPilot.Spike;

internal readonly record struct WindowInfo(IntPtr Handle, string Title, string ProcessName);

internal static class WindowScanner
{
    internal static List<WindowInfo> VisibleTitledWindows()
    {
        List<WindowInfo> found = new();

        NativeWindows.EnumWindows(
            (window, _) =>
            {
                if (!NativeWindows.IsWindowVisible(window))
                {
                    return true;
                }

                string title = TitleOf(window);
                if (title.Length == 0)
                {
                    return true;
                }

                found.Add(new WindowInfo(window, title, ProcessNameOf(window)));
                return true;
            },
            IntPtr.Zero);

        return found;
    }

    internal static WindowInfo Describe(IntPtr window)
    {
        return new WindowInfo(window, TitleOf(window), ProcessNameOf(window));
    }

    private static string TitleOf(IntPtr window)
    {
        int length = NativeWindows.GetWindowTextLength(window);
        if (length == 0)
        {
            return string.Empty;
        }

        StringBuilder buffer = new(length + 1);
        NativeWindows.GetWindowText(window, buffer, buffer.Capacity);
        return buffer.ToString();
    }

    private static string ProcessNameOf(IntPtr window)
    {
        NativeWindows.GetWindowThreadProcessId(window, out int processId);

        // A window can outlive its process between the enumeration and this call,
        // and a protected process refuses the open outright.
        try
        {
            using Process process = Process.GetProcessById(processId);
            return process.ProcessName;
        }
        catch (Exception)
        {
            return "unknown";
        }
    }
}
