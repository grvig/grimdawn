namespace GDPilot.Loopback;

internal static class Program
{
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
        log.WriteLine("ready");

        using Form window = new();
        window.Text = "GDPilot Loopback";
        window.Width = 320;
        window.Height = 120;
        Application.Run(window);
        return 0;
    }
}
