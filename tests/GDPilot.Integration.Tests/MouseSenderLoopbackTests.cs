using System.Text.RegularExpressions;
using GDPilot.Output;

namespace GDPilot.Integration.Tests;

[Collection(LoopbackCollection.Name)]
public sealed class MouseSenderLoopbackTests
{
    private readonly LoopbackFixture loopback;
    private readonly MouseSender sender = new();

    public MouseSenderLoopbackTests(LoopbackFixture loopback)
    {
        this.loopback = loopback;
    }

    [Fact]
    public void RelativeMoveTravelsInTheRequestedDirection()
    {
        (int X, int Y) start = sender.CursorPosition();
        int before = loopback.LineCount();

        sender.MoveRelative(40, 25);

        // Pointer acceleration can scale a relative move, so only direction is exact.
        (int X, int Y) landed = PointOf(loopback.WaitForNew(before, lines => lines.Count >= 1)[0], "move");
        Assert.True(landed.X > start.X, $"Expected x beyond {start.X}, got {landed.X}.");
        Assert.True(landed.Y > start.Y, $"Expected y beyond {start.Y}, got {landed.Y}.");
    }

    [Fact]
    public void AbsoluteMoveLandsOnTheRequestedPixel()
    {
        int before = loopback.LineCount();

        sender.MoveAbsolute(200, 150);

        (int X, int Y) landed = PointOf(loopback.WaitForNew(before, lines => lines.Count >= 1)[0], "move");
        Assert.InRange(landed.X, 199, 201);
        Assert.InRange(landed.Y, 149, 151);
    }

    [Theory]
    [InlineData(MouseButton.Left, "left")]
    [InlineData(MouseButton.Right, "right")]
    public void ClickSendsDownThenUp(MouseButton button, string name)
    {
        int before = loopback.LineCount();

        sender.Click(button);

        List<string> recorded = loopback.WaitForNew(before, lines => lines.Count >= 2);
        Assert.StartsWith($"mouse {name}down", recorded[0]);
        Assert.StartsWith($"mouse {name}up", recorded[1]);
    }

    private static (int X, int Y) PointOf(string line, string expectedEvent)
    {
        Match match = Regex.Match(line, @"^mouse (\w+) x=(-?\d+) y=(-?\d+)$");
        Assert.True(match.Success, $"Unrecognised loopback line: {line}");
        Assert.Equal(expectedEvent, match.Groups[1].Value);
        return (int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value));
    }
}
