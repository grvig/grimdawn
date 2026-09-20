using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GDPilot.Integration.Tests;

/// <summary>
/// Runs one loopback window for a whole test class and exposes what it has
/// recorded. Its hooks swallow injected events, so nothing reaches other windows.
/// </summary>
public sealed class LoopbackFixture : IDisposable
{
    private readonly Process process;
    private readonly string logPath;

    /// <summary>The window's client area in screen pixels, as the window reported it.</summary>
    public (int X, int Y, int Width, int Height) ClientBounds { get; }

    /// <summary>The colour the window paints its client area.</summary>
    public (int Blue, int Green, int Red) Background { get; }

    public LoopbackFixture()
    {
        GDPilot.Output.DpiAwareness.EnablePerMonitor();
        logPath = Path.Combine(Path.GetTempPath(), $"gdpilot-loopback-{Guid.NewGuid():N}.log");
        process = Process.Start(LoopbackExecutable(), $"\"{logPath}\"");

        List<string> header = WaitFor(lines => lines.Contains("ready"), TimeSpan.FromSeconds(15));
        Match bounds = Find(header, @"^client x=(-?\d+) y=(-?\d+) w=(\d+) h=(\d+)$");
        Match colour = Find(header, @"^background b=(\d+) g=(\d+) r=(\d+)$");

        ClientBounds = (Number(bounds, 1), Number(bounds, 2), Number(bounds, 3), Number(bounds, 4));
        Background = (Number(colour, 1), Number(colour, 2), Number(colour, 3));
    }

    private static Match Find(List<string> lines, string pattern)
    {
        foreach (string line in lines)
        {
            Match match = Regex.Match(line, pattern);

            if (match.Success)
            {
                return match;
            }
        }

        throw new InvalidOperationException($"No loopback line matched {pattern}. Recorded:\n{string.Join("\n", lines)}");
    }

    private static int Number(Match match, int group)
    {
        return int.Parse(match.Groups[group].Value);
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
