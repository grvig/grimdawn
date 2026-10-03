using GDPilot.Core;

namespace GDPilot.Core.Tests;

public sealed class ReplayGamepadSourceTests
{
    private static readonly GamepadTrace Trace = GamepadTrace.Parse("""
        { "entries": [
            { "at": 100, "buttons": "A" },
            { "at": 200, "buttons": "B", "rightX": -0.75 }
        ] }
        """);

    private TimeSpan now;

    [Fact]
    public void PlaybackFollowsTheClock()
    {
        ReplayGamepadSource source = new(Trace, () => now);

        Assert.Equal(GamepadSnapshot.Rest, ReadAt(source, 0));
        Assert.Equal(GamepadButtons.A, ReadAt(source, 100).Buttons);
        Assert.Equal(GamepadButtons.A, ReadAt(source, 199).Buttons);
        Assert.Equal(GamepadButtons.B, ReadAt(source, 200).Buttons);
        Assert.Equal(-0.75, ReadAt(source, 5000).RightStick.X);
    }

    [Fact]
    public void FinishesWhenTheLastEntryBegins()
    {
        ReplayGamepadSource source = new(Trace, () => now);

        now = TimeSpan.FromMilliseconds(199);
        Assert.False(source.IsFinished);

        now = TimeSpan.FromMilliseconds(200);
        Assert.True(source.IsFinished);
        Assert.True(source.IsConnected);
    }

    private GamepadSnapshot ReadAt(ReplayGamepadSource source, int milliseconds)
    {
        now = TimeSpan.FromMilliseconds(milliseconds);
        return source.Read();
    }
}
