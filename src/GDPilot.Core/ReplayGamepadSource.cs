namespace GDPilot.Core;

/// <summary>
/// Plays a trace back against a clock the caller owns. A test steps that clock by
/// hand, which makes a replay exactly repeatable, unlike one paced by real time.
/// </summary>
public sealed class ReplayGamepadSource : IGamepadSource
{
    private readonly GamepadTrace trace;
    private readonly Func<TimeSpan> elapsed;

    public ReplayGamepadSource(GamepadTrace trace, Func<TimeSpan> elapsed)
    {
        this.trace = trace;
        this.elapsed = elapsed;
    }

    public bool IsConnected => true;

    public bool IsFinished => elapsed().TotalMilliseconds >= trace.Entries[^1].At;

    /// <summary>The latest entry that has begun. Before the first one, the pad is at rest.</summary>
    public GamepadSnapshot Read()
    {
        double now = elapsed().TotalMilliseconds;
        GamepadSnapshot current = GamepadSnapshot.Rest;

        foreach (TraceEntry entry in trace.Entries)
        {
            if (entry.At > now)
            {
                break;
            }

            current = entry.ToSnapshot();
        }

        return current;
    }
}
