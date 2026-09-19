using GDPilot.Core;

namespace GDPilot.Core.Tests;

public sealed class FrameDifferenceTests
{
    private const int Width = 10;
    private const int Height = 10;

    [Fact]
    public void IdenticalFramesScoreZero()
    {
        byte[] frame = Filled(40);

        double score = FrameDifference.MeanAbsolute(frame, frame, Width, Height, 0.5);

        Assert.Equal(0, score);
    }

    [Fact]
    public void BlackToWhiteScoresTheFullRange()
    {
        double score = FrameDifference.MeanAbsolute(Filled(0), Filled(255), Width, Height, 0.5);

        Assert.Equal(255, score);
    }

    [Fact]
    public void ChangesOutsideTheCentralRegionAreIgnored()
    {
        byte[] before = Filled(0);
        byte[] after = Filled(0);

        // Corner pixel sits inside the margin that a 0.5 fraction discards.
        after[0] = 255;
        after[1] = 255;
        after[2] = 255;

        double score = FrameDifference.MeanAbsolute(before, after, Width, Height, 0.5);

        Assert.Equal(0, score);
    }

    [Fact]
    public void AlphaChannelIsIgnored()
    {
        byte[] before = Filled(0);
        byte[] after = Filled(0);

        for (int offset = 3; offset < after.Length; offset += 4)
        {
            after[offset] = 255;
        }

        Assert.Equal(0, FrameDifference.MeanAbsolute(before, after, Width, Height, 1.0));
    }

    [Fact]
    public void MismatchedFrameSizesAreRejected()
    {
        Assert.Throws<ArgumentException>(() => FrameDifference.MeanAbsolute(new byte[4], Filled(0), Width, Height, 0.5));
    }

    private static byte[] Filled(byte value)
    {
        byte[] frame = new byte[Width * Height * 4];
        Array.Fill(frame, value);
        return frame;
    }
}
