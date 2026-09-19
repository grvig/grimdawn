using System.Diagnostics;

namespace GDPilot.Integration.Tests;

/// <summary>
/// Runs one loopback window for a whole test class and exposes what it has
/// recorded. Its hooks swallow injected events, so nothing reaches other windows.
/// </summary>
public sealed class LoopbackFixture : IDisposable
{
    private readonly Process process;
    private readonly string logPath;

    public LoopbackFixture()
    {
        GDPilot.Output.DpiAwareness.EnablePerMonitor();
        logPath = Path.Combine(Path.GetTempPath(), $"gdpilot-loopback-{Guid.NewGuid():N}.log");
        process = Process.Start(LoopbackExecutable(), $"\"{logPath}\"");
        WaitFor(lines => lines.Contains("ready"), TimeSpan.FromSeconds(15));
    }

    public int LineCount()
    {
        return ReadLines().Count;
    }

    /// <summary>Waits for lines recorded after <paramref name="skip"/> to satisfy the condition.</summary>
    public List<string> WaitForNew(int skip, Func<List<string>, bool> condition)
    {
        return WaitFor(lines => condition(lines.Skip(skip).ToList()), TimeSpan.FromSeconds(3)).Skip(skip).ToList();
    }

    public void Dispose()
    {
        if (!process.HasExited)
        {
            process.Kill();
            process.WaitForExit();
        }

        process.Dispose();
        File.Delete(logPath);
    }

    private List<string> WaitFor(Func<List<string>, bool> condition, TimeSpan timeout)
    {
        Stopwatch clock = Stopwatch.StartNew();
        List<string> lines = ReadLines();

        while (!condition(lines))
        {
            if (clock.Elapsed > timeout)
            {
                throw new TimeoutException("Loopback log never matched. Recorded:\n" + string.Join("\n", lines));
            }

            Thread.Sleep(20);
            lines = ReadLines();
        }

        return lines;
    }

    private List<string> ReadLines()
    {
        if (!File.Exists(logPath))
        {
            return new List<string>();
        }

        using FileStream stream = new(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader reader = new(stream);
        return reader.ReadToEnd().Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }

    private static string LoopbackExecutable()
    {
        // The project reference copies the executable next to this assembly.
        return Path.Combine(AppContext.BaseDirectory, "GDPilot.Loopback.exe");
    }
}
