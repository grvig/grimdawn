using GDPilot.Core;

namespace GDPilot.Core.Tests;

public sealed class ProbeVerdictTests
{
    [Theory]
    [InlineData(20.0, 0.5)]
    [InlineData(6.0, 2.0)]
    [InlineData(30.0, 10.0)]
    public void ClearMovementAboveTheIdleNoisePasses(double moved, double idle)
    {
        Assert.Equal(ProbeOutcome.Pass, ProbeVerdict.Classify(moved, idle));
    }

    [Theory]
    [InlineData(0.0, 0.0)]
    [InlineData(1.0, 0.0)]
    [InlineData(3.5, 2.0)]
    [InlineData(15.0, 10.0)]
    public void MovementIndistinguishableFromIdleFails(double moved, double idle)
    {
        Assert.Equal(ProbeOutcome.Fail, ProbeVerdict.Classify(moved, idle));
    }

    [Theory]
    [InlineData(3.0, 0.0)]
    [InlineData(5.0, 2.0)]
    [InlineData(20.0, 10.0)]
    public void TheBandBetweenIsInconclusive(double moved, double idle)
    {
        Assert.Equal(ProbeOutcome.Inconclusive, ProbeVerdict.Classify(moved, idle));
    }
}
