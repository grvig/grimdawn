using GDPilot.Output;

namespace GDPilot.Spike;

internal static class Program
{
    private static int Main(string[] args)
    {
        // Before anything reads or sends a screen coordinate, or they are all
        // wrong by the display's scale factor.
        DpiAwareness.EnablePerMonitor();

        if (args.Contains("--windows"))
        {
            ListWindows();
            return 0;
        }

        if (args.Contains("--target"))
        {
            ShowTarget();
            return 0;
        }

        Console.WriteLine("GDPilot feasibility probe.");
        Console.WriteLine();
        Console.WriteLine("  --windows   List visible top-level windows with their process names.");
        Console.WriteLine("  --target    Count down, then describe whichever window is in front.");
        return 0;
    }

    private static void ListWindows()
    {
        List<WindowInfo> windows = WindowScanner.VisibleTitledWindows();
        Console.WriteLine($"{windows.Count} visible top-level windows.");
        Console.WriteLine();

        foreach (WindowInfo window in windows)
        {
            Console.WriteLine($"  {window.ProcessName,-24} {window.Title}");
        }
    }

    private static void ShowTarget()
    {
        ProbeTarget target = TargetPicker.FromForegroundWindow(5);
        Console.WriteLine();
        Console.WriteLine($"Process:     {target.ProcessName}");
        Console.WriteLine($"Title:       {target.Title}");
        Console.WriteLine($"Client area: {target.Width}×{target.Height} at {target.X},{target.Y}");
    }
}
