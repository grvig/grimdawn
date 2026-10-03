using System.Text.Json;
using System.Text.Json.Serialization;

namespace GDPilot.Core;

/// <summary>
/// One recorded pad state and the moment it began. Anything a fixture leaves out
/// is at rest, so a hand-written trace only has to state what changes.
/// </summary>
public sealed class TraceEntry
{
    public int At { get; init; }
    public GamepadButtons Buttons { get; init; }
    public double LeftX { get; init; }
    public double LeftY { get; init; }
    public double RightX { get; init; }
    public double RightY { get; init; }
    public double LeftTrigger { get; init; }
    public double RightTrigger { get; init; }

    public GamepadSnapshot ToSnapshot()
    {
        return new GamepadSnapshot(Buttons, new StickPosition(LeftX, LeftY), new StickPosition(RightX, RightY), LeftTrigger, RightTrigger);
    }
}

/// <summary>A scripted controller performance, ordered by time, loaded from JSON.</summary>
public sealed class GamepadTrace
{
    // Named buttons such as "A, LeftBumper" keep fixtures readable. Plain numbers still parse.
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() },
    };

    public IReadOnlyList<TraceEntry> Entries { get; }

    private GamepadTrace(IReadOnlyList<TraceEntry> entries)
    {
        Entries = entries;
    }

    public static GamepadTrace Parse(string json)
    {
        TraceDocument? document = JsonSerializer.Deserialize<TraceDocument>(json, Options);

        if (document is null || document.Entries.Count == 0)
        {
            throw new FormatException("A trace needs at least one entry.");
        }

        for (int index = 0; index < document.Entries.Count; index++)
        {
            Validate(document.Entries, index);
        }

        return new GamepadTrace(document.Entries);
    }

    private static void Validate(List<TraceEntry> entries, int index)
    {
        TraceEntry entry = entries[index];

        if (index > 0 && entry.At < entries[index - 1].At)
        {
            throw new FormatException($"Entry {index} at {entry.At} ms is earlier than the entry before it.");
        }

        double[] sticks = { entry.LeftX, entry.LeftY, entry.RightX, entry.RightY };
        double[] triggers = { entry.LeftTrigger, entry.RightTrigger };

        if (sticks.Any(value => value < -1 || value > 1) || triggers.Any(value => value < 0 || value > 1))
        {
            throw new FormatException($"Entry {index} has an axis outside its range.");
        }
    }

    private sealed class TraceDocument
    {
        public List<TraceEntry> Entries { get; init; } = new();
    }
}
