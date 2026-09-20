using System.Text;
using GDPilot.Core;

namespace GDPilot.Spike;

internal static class SpikeResults
{
    internal static string Compose(ProbeTarget target, List<ProbeRun> runs)
    {
        ProbeRun last = runs[^1];
        StringBuilder report = new();

        report.AppendLine("# Spike results");
        report.AppendLine();
        report.AppendLine($"Run on {DateTime.Now:yyyy-MM-dd HH:mm}.");
        report.AppendLine();
        report.AppendLine($"Target: {target.ProcessName}, \"{target.Title}\", client {target.Width}×{target.Height} at {target.X},{target.Y}.");
        report.AppendLine();
        report.AppendLine("| Rung | Idle score | Moved score | Outcome |");
        report.AppendLine("| --- | --- | --- | --- |");

        foreach (ProbeRun run in runs)
        {
            report.AppendLine($"| {run.Rung} | {run.IdleScore:F2} | {run.MovedScore:F2} | {run.Outcome} |");
        }

        report.AppendLine();
        report.AppendLine($"Cursor moved by {last.CursorDeltaX},{last.CursorDeltaY} after a 120 pixel relative move.");
        report.AppendLine();
        report.AppendLine($"## Verdict: {last.Outcome}");
        report.AppendLine();
        report.AppendLine(Guidance(last.Outcome));
        return report.ToString();
    }

    private static string Guidance(ProbeOutcome outcome)
    {
        if (outcome == ProbeOutcome.Pass)
        {
            return "The game acts on synthesized input. Record which rung passed and carry on.";
        }

        if (outcome == ProbeOutcome.Inconclusive)
        {
            return "The movement score did not clear the idle noise. Stand somewhere still, away from rain, fire and moving water, then run it again.";
        }

        return "No rung moved the character. Remaining rungs are manual: run the game windowed rather than borderless, then try again elevated. If both fail, the project needs a kernel level input driver, which is a different project.";
    }
}
