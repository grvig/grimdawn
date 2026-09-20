using GDPilot.Core;
using GDPilot.Output;
using GDPilot.Vision;

namespace GDPilot.Spike;

internal readonly record struct ProbeRun(
    string Rung,
    double IdleScore,
    double MovedScore,
    ProbeOutcome Outcome,
    int CursorDeltaX,
    int CursorDeltaY);

internal static class InputProbe
{
    // The outer band of the client area holds the HUD, which animates on its own.
    private const double CentralFraction = 0.6;

    internal static ProbeRun Run(ProbeTarget target, string rung, KeyInjectionMode mode, TimeSpan hold)
    {
        double idleScore = ScoreIdle(target, hold);
        double movedScore = ScoreMovement(target, mode, hold);
        (int X, int Y) cursorDelta = ScoreCursor();

        return new ProbeRun(rung, idleScore, movedScore, ProbeVerdict.Classify(movedScore, idleScore), cursorDelta.X, cursorDelta.Y);
    }

    /// <summary>How much the scene changes on its own, with nothing injected.</summary>
    private static double ScoreIdle(ProbeTarget target, TimeSpan hold)
    {
        CapturedFrame before = Capture(target);
        Thread.Sleep(hold);
        CapturedFrame after = Capture(target);
        return Compare(before, after);
    }

    private static double ScoreMovement(ProbeTarget target, KeyInjectionMode mode, TimeSpan hold)
    {
        KeySender keys = new(mode);
        CapturedFrame before = Capture(target);

        keys.Down(KeyCode.W);

        try
        {
            Thread.Sleep(hold);
        }
        finally
        {
            // A key left down survives this process, so release it even on failure.
            keys.Up(KeyCode.W);
        }

        CapturedFrame after = Capture(target);
        return Compare(before, after);
    }

    private static (int X, int Y) ScoreCursor()
    {
        MouseSender mouse = new();
        (int X, int Y) before = mouse.CursorPosition();
        mouse.MoveRelative(120, 0);
        Thread.Sleep(50);
        (int X, int Y) after = mouse.CursorPosition();
        return (after.X - before.X, after.Y - before.Y);
    }

    private static CapturedFrame Capture(ProbeTarget target)
    {
        return ScreenCapture.Region(target.X, target.Y, target.Width, target.Height);
    }

    private static double Compare(CapturedFrame before, CapturedFrame after)
    {
        return FrameDifference.MeanAbsolute(before.Pixels, after.Pixels, before.Width, before.Height, CentralFraction);
    }
}
