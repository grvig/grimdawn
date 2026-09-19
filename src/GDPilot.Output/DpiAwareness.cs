using System.Runtime.InteropServices;

namespace GDPilot.Output;

public static class DpiAwareness
{
    // DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2, a pseudo-handle defined as -4.
    private static readonly IntPtr PerMonitorAwareV2 = new(-4);

    /// <summary>
    /// Opts the process into physical pixel coordinates. An unaware process on a
    /// scaled display is shown a shrunken virtual screen, so absolute moves and
    /// cursor reads are off by the scale factor: 125% turns pixel 200 into 250.
    /// Must run before any window is created. A second call is harmless.
    /// </summary>
    public static void EnablePerMonitor()
    {
        SetProcessDpiAwarenessContext(PerMonitorAwareV2);
    }

    [DllImport("user32.dll")]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr context);
}
