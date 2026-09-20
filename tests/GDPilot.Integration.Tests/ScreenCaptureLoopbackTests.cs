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

        CapturedFrame frame = ScreenCapture.Region(area.X, area.Y, area.Width, area.Height);

        Assert.Equal(area.Width * area.Height * 4, frame.Pixels.Length);
        Assert.Equal(area.Width, frame.Width);
        Assert.Equal(area.Height, frame.Height);

        double matching = FractionMatchingBackground(frame);
        Assert.True(matching > 0.95, $"Only {matching:P1} of the captured pixels were the window's colour.");
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
