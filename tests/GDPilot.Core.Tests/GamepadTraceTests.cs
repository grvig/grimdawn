using GDPilot.Core;

namespace GDPilot.Core.Tests;

public sealed class GamepadTraceTests
{
    [Fact]
    public void NamedButtonsAndOmittedAxesParse()
    {
        GamepadTrace trace = GamepadTrace.Parse("""
            { "entries": [
                { "at": 0 },
                { "at": 120, "buttons": "A, LeftBumper", "leftX": 0.5, "rightTrigger": 1 }
            ] }
            """);

        GamepadSnapshot pressed = trace.Entries[1].ToSnapshot();

        Assert.Equal(GamepadSnapshot.Rest, trace.Entries[0].ToSnapshot());
        Assert.Equal(120, trace.Entries[1].At);
        Assert.True(pressed.IsPressed(GamepadButtons.A | GamepadButtons.LeftBumper));
        Assert.False(pressed.IsPressed(GamepadButtons.B));
        Assert.Equal(new StickPosition(0.5, 0), pressed.LeftStick);
        Assert.Equal(1, pressed.RightTrigger);
    }

    [Fact]
    public void NumericBitfieldParses()
    {
        GamepadTrace trace = GamepadTrace.Parse("""{ "entries": [ { "at": 0, "buttons": 17 } ] }""");

        Assert.Equal(GamepadButtons.A | GamepadButtons.LeftBumper, trace.Entries[0].Buttons);
    }

    [Theory]
    [InlineData("""{ "entries": [] }""")]
    [InlineData("""{ "entries": [ { "at": 50 }, { "at": 10 } ] }""")]
    [InlineData("""{ "entries": [ { "at": 0, "leftX": 1.5 } ] }""")]
    [InlineData("""{ "entries": [ { "at": 0, "leftTrigger": -0.1 } ] }""")]
    public void MalformedTracesAreRejected(string json)
    {
        Assert.Throws<FormatException>(() => GamepadTrace.Parse(json));
    }
}
