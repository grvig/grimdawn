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
        loopback.RaiseWindow();
        CapturedFrame frame = ScreenCapture.Region(area.X, area.Y, area.Width, area.Height);
        double matching = FractionMatchingBackground(frame);

        // The window reports itself shown before its first paint is guaranteed to
        // reach the screen, and another window can rise over it at any moment.
        // Retrying with the window raised again tests the capture, not the desktop.
        while (matching <= 0.95 && clock.Elapsed < TimeSpan.FromSeconds(2))
        {
            Thread.Sleep(50);
            loopback.RaiseWindow();
            frame = ScreenCapture.Region(area.X, area.Y, area.Width, area.Height);
            matching = FractionMatchingBackground(frame);
        }

        Assert.Equal(area.Width * area.Height * 4, frame.Pixels.Length);
        Assert.Equal(area.Width, frame.Width);
        Assert.Equal(area.Height, frame.Height);

        // This test has failed intermittently for reasons not yet found, so a
        // failure reports what was actually on screen rather than only that it differed.
        if (matching <= 0.95)
        {
            string owner = ScreenOwner.Describe(area.X + area.Width / 2, area.Y + area.Height / 2);
            IntPtr handle = loopback.WindowHandle;
            Assert.Fail($"Only {matching:P1} of the captured pixels were the window's colour after {clock.Elapsed.TotalSeconds:F1}s. "
                + $"Most common colour {DominantColour(frame)}. Window at the client centre: {owner}. Bounds {area}. "
                + $"Loopback window {handle}, topmost {ScreenOwner.IsTopmost(handle)}, raise succeeded {loopback.RaiseWindow()}.");
        }
    }

    private static string DominantColour(CapturedFrame frame)
    {
        Dictionary<int, int> counts = new();

        for (int offset = 0; offset < frame.Pixels.Length; offset += 4)
        {
            int colour = frame.Pixels[offset] | frame.Pixels[offset + 1] << 8 | frame.Pixels[offset + 2] << 16;
            counts[colour] = counts.GetValueOrDefault(colour) + 1;
        }

        KeyValuePair<int, int> top = counts.MaxBy(pair => pair.Value);
        double share = (double)top.Value / (frame.Width * frame.Height);
        return $"b={top.Key & 0xFF} g={top.Key >> 8 & 0xFF} r={top.Key >> 16 & 0xFF} ({share:P0})";
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
