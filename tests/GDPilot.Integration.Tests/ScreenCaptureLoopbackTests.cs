using System.Diagnostics;
using GDPilot.Vision;

namespace GDPilot.Integration.Tests;

[Collection(LoopbackCollection.Name)]
public sealed class ScreenCaptureLoopbackTests
{
    private readonly LoopbackFixture loopback;

    public ScreenCaptureLoopbackTests(LoopbackFixture loopback)
    {
        this.loopback = loopback;
    }

    [Fact]
    public void CapturingTheLoopbackWindowReturnsItsKnownColour()
    {
        (int X, int Y, int Width, int Height) area = loopback.ClientBounds;
        Stopwatch clock = Stopwatch.StartNew();
        CapturedFrame frame = ScreenCapture.Region(area.X, area.Y, area.Width, area.Height);
        double matching = FractionMatchingBackground(frame);

        // The window reports itself shown before its first paint reaches the
        // screen, and on a cold start that gap is long enough to capture what was
        // underneath. Waiting for the colour to appear tests the capture, not the race.
        while (matching <= 0.95 && clock.Elapsed < TimeSpan.FromSeconds(2))
        {
            Thread.Sleep(50);
            frame = ScreenCapture.Region(area.X, area.Y, area.Width, area.Height);
            matching = FractionMatchingBackground(frame);
        }

        Assert.Equal(area.Width * area.Height * 4, frame.Pixels.Length);
        Assert.Equal(area.Width, frame.Width);
        Assert.Equal(area.Height, frame.Height);
        Assert.True(matching > 0.95, $"Only {matching:P1} of the captured pixels were the window's colour after {clock.Elapsed.TotalSeconds:F1}s.");
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(10, 0)]
    [InlineData(-4, 10)]
    public void EmptyRegionsAreRejected(int width, int height)
    {
        Assert.Throws<ArgumentException>(() => ScreenCapture.Region(0, 0, width, height));
    }

    private double FractionMatchingBackground(CapturedFrame frame)
    {
        (int Blue, int Green, int Red) expected = loopback.Background;
        int matched = 0;

        for (int offset = 0; offset < frame.Pixels.Length; offset += 4)
        {
            if (frame.Pixels[offset] == expected.Blue && frame.Pixels[offset + 1] == expected.Green && frame.Pixels[offset + 2] == expected.Red)
            {
                matched++;
            }
        }

        return (double)matched / (frame.Width * frame.Height);
    }
}
