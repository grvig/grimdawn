namespace GDPilot.Spike;

internal static class Program
{
    private static int Main(string[] args)
    {
        if (args.Contains("--windows"))
        {
            ListWindows();
            return 0;
        }

        Console.WriteLine("GDPilot feasibility probe.");
        Console.WriteLine();
        Console.WriteLine("  --windows   List visible top-level windows with their process names.");
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
}
