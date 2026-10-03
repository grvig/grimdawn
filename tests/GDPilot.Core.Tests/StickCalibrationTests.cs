using GDPilot.Core;

namespace GDPilot.Core.Tests;

public sealed class StickCalibrationTests
{
    private const double Tolerance = 1e-9;

    [Fact]
    public void PerfectlyCentredStickCalibratesToZero()
    {
        StickPosition[] samples = { StickPosition.Centre, StickPosition.Centre, StickPosition.Centre };

        bool found = StickCalibration.TryFindCentre(samples, out StickPosition centre);

        Assert.True(found);
        Assert.Equal(StickPosition.Centre, centre);
    }

    [Fact]
    public void DriftingStickCalibratesToTheMeanOfItsJitter()
    {
        StickPosition[] samples =
        {
            new(0.11, -0.04),
            new(0.09, -0.06),
            new(0.10, -0.05),
            new(0.10, -0.05),
        };

        bool found = StickCalibration.TryFindCentre(samples, out StickPosition centre);

        Assert.True(found);
        Assert.Equal(0.10, centre.X, Tolerance);
        Assert.Equal(-0.05, centre.Y, Tolerance);
    }

    [Fact]
    public void StickHeldOverDuringStartupIsRejected()
    {
        StickPosition[] samples = { new(0.8, 0.0), new(0.81, 0.01), new(0.79, -0.01) };

        bool found = StickCalibration.TryFindCentre(samples, out StickPosition centre);

        Assert.False(found);
        Assert.Equal(StickPosition.Centre, centre);
    }

    [Fact]
    public void StickMovedDuringSamplingIsRejected()
    {
        // The mean lands near centre, which alone would pass. The spread gives it away.
        StickPosition[] samples = { new(-0.25, 0.0), new(0.0, 0.0), new(0.25, 0.0) };

        bool found = StickCalibration.TryFindCentre(samples, out _);

        Assert.False(found);
    }

    [Fact]
    public void NoSamplesFindsNoCentre()
    {
        bool found = StickCalibration.TryFindCentre(Array.Empty<StickPosition>(), out _);

        Assert.False(found);
    }
}
