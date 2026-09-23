using GDPilot.Core;

namespace GDPilot.Core.Tests;

public sealed class StickShapingTests
{
    private const double Deadzone = 0.2;
    private const double Tolerance = 1e-9;

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(0.1, 0.1)]
    [InlineData(-0.2, 0.0)]
    [InlineData(0.0, 0.19)]
    public void DeflectionInsideTheDeadzoneReadsAsRest(double x, double y)
    {
        StickPosition shaped = StickShaping.ApplyRadialDeadzone(new StickPosition(x, y), StickPosition.Centre, Deadzone);

        Assert.Equal(StickPosition.Centre, shaped);
    }

    [Fact]
    public void OutputRisesFromZeroAtTheDeadzoneEdge()
    {
        StickPosition shaped = StickShaping.ApplyRadialDeadzone(new StickPosition(0.21, 0), StickPosition.Centre, Deadzone);

        Assert.InRange(shaped.Magnitude, 0, 0.02);
    }

    [Fact]
    public void FullDeflectionReachesFullMagnitude()
    {
        StickPosition shaped = StickShaping.ApplyRadialDeadzone(new StickPosition(0, -1), StickPosition.Centre, Deadzone);

        Assert.Equal(1.0, shaped.Magnitude, Tolerance);
        Assert.Equal(-1.0, shaped.Y, Tolerance);
    }

    [Fact]
    public void ShallowDiagonalKeepsItsDirection()
    {
        StickPosition raw = new(0.6, 0.15);

        StickPosition shaped = StickShaping.ApplyRadialDeadzone(raw, StickPosition.Centre, Deadzone);

        Assert.Equal(Math.Atan2(raw.Y, raw.X), Math.Atan2(shaped.Y, shaped.X), Tolerance);
    }

    [Fact]
    public void SquareGateCornerIsClampedToFullMagnitude()
    {
        StickPosition shaped = StickShaping.ApplyRadialDeadzone(new StickPosition(1, 1), StickPosition.Centre, Deadzone);

        Assert.Equal(1.0, shaped.Magnitude, Tolerance);
    }

    [Fact]
    public void DriftedRestingPositionReadsAsRest()
    {
        StickPosition drift = new(0.12, -0.08);

        StickPosition shaped = StickShaping.ApplyRadialDeadzone(drift, drift, Deadzone);

        Assert.Equal(StickPosition.Centre, shaped);
    }

    [Fact]
    public void DriftedStickStillReachesFullMagnitudeAwayFromTheDrift()
    {
        StickPosition drift = new(0.1, 0);

        StickPosition shaped = StickShaping.ApplyRadialDeadzone(new StickPosition(-1, 0), drift, Deadzone);

        Assert.Equal(1.0, shaped.Magnitude, Tolerance);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.0)]
    public void DeadzoneOutsideItsRangeIsRejected(double deadzone)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => StickShaping.ApplyRadialDeadzone(StickPosition.Centre, StickPosition.Centre, deadzone));
    }

    [Fact]
    public void CentreTooFarOutToLeaveTravelIsRejected()
    {
        Assert.Throws<ArgumentException>(() => StickShaping.ApplyRadialDeadzone(StickPosition.Centre, new StickPosition(0.9, 0), Deadzone));
    }
}
