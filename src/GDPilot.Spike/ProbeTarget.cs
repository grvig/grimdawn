namespace GDPilot.Spike;

internal readonly record struct ProbeTarget(IntPtr Window, string Title, string ProcessName, int X, int Y, int Width, int Height);

internal static class TargetPicker
{
    /// <summary>
    /// Counts down, then takes whatever window is in front. The probe must not
    /// hardcode a title or an executable name, and asking the person to bring the
    /// game forward identifies it without guessing.
    /// </summary>
    internal static ProbeTarget FromForegroundWindow(int countdownSeconds)
    {
        Console.WriteLine($"Switch to the game now. Taking the front window in {countdownSeconds} seconds.");

        for (int remaining = countdownSeconds; remaining > 0; remaining--)
        {
            Console.WriteLine($"  {remaining}...");
            Thread.Sleep(1000);
        }

        IntPtr window = NativeWindows.GetForegroundWindow();

        if (window == IntPtr.Zero)
        {
            throw new InvalidOperationException("No window was in the foreground.");
        }

        return Describe(window);
    }

    private static ProbeTarget Describe(IntPtr window)
    {
        if (!NativeWindows.GetClientRect(window, out NativeWindows.Rect client))
        {
            throw new InvalidOperationException("Could not measure the window's client area.");
        }

        // GetClientRect is window-relative, so its origin must be mapped to the screen.
        NativeWindows.Point origin = new();
        origin.X = client.Left;
        origin.Y = client.Top;

        if (!NativeWindows.ClientToScreen(window, ref origin))
        {
            throw new InvalidOperationException("Could not place the client area on the screen.");
        }

        WindowInfo info = WindowScanner.Describe(window);
        int width = client.Right - client.Left;
        int height = client.Bottom - client.Top;
        return new ProbeTarget(window, info.Title, info.ProcessName, origin.X, origin.Y, width, height);
    }
}
