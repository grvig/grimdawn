using GDPilot.Core;
using GDPilot.Output;

namespace GDPilot.Spike;

internal static class FeasibilityLadder
{
    private static readonly (string Rung, KeyInjectionMode Mode)[] Rungs =
    {
        ("virtual key", KeyInjectionMode.VirtualKey),
        ("scancode", KeyInjectionMode.ScanCode),
    };

    /// <summary>
    /// Runs each rung until one passes. An inconclusive rung is retried once with a
    /// longer hold, since weather and idle animation can blur a short sample.
    /// </summary>
    internal static List<ProbeRun> Climb(ProbeTarget target, TimeSpan hold)
    {
        List<ProbeRun> runs = new();

        foreach ((string rung, KeyInjectionMode mode) in Rungs)
        {
            ProbeRun run = InputProbe.Run(target, rung, mode, hold);
            runs.Add(run);

            if (run.Outcome == ProbeOutcome.Inconclusive)
            {
                run = InputProbe.Run(target, rung + ", longer sample", mode, hold + hold);
                runs.Add(run);
            }

            if (run.Outcome == ProbeOutcome.Pass)
            {
                break;
            }
        }

        return runs;
    }
}
