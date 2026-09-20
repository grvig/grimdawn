using System.Drawing;

namespace GDPilot.Loopback;

internal static class Program
{
    // A colour no ordinary window paints, so a capture test can tell this window
    // apart from whatever it happens to be drawn over.
    private static readonly Color Background = Color.FromArgb(255, 0, 255);

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.Error.WriteLine("Usage: GDPilot.Loopback <log file>");
            return 2;
        }

        ApplicationConfiguration.Initialize();

        // Shared read access lets a test tail the log while this process still owns it.
        FileStream stream = new(args[0], FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        using StreamWriter log = new(stream);
        log.AutoFlush = true;

        // Hooks deliver on the installing thread, so they must be installed on the
        // same thread that then runs the message loop.
        using HookRecorder recorder = new(log);

        using Form window = new();
        window.Text = "GDPilot Loopback";
        window.Width = 320;
        window.Height = 240;
        window.BackColor = Background;
        window.FormBorderStyle = FormBorderStyle.FixedToolWindow;

        // Topmost keeps the client area unobscured, which a capture test depends on.
        window.TopMost = true;
        window.Shown += (_, _) => Announce(log, window);

        Application.Run(window);
        return 0;
    }

    private static void Announce(StreamWriter log, Form window)
    {
        Point origin = window.PointToScreen(Point.Empty);
        log.WriteLine($"client x={origin.X} y={origin.Y} w={window.ClientSize.Width} h={window.ClientSize.Height}");
        log.WriteLine($"background b={Background.B} g={Background.G} r={Background.R}");

        // Written last so a reader that waits for it knows the rest is already there.
        log.WriteLine("ready");
    }
}
