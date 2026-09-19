namespace GDPilot.Core;

public static class FrameDifference
{
    /// <summary>
    /// Mean absolute per-channel difference between two 32-bit BGRA frames, over a
    /// centred region. The edges hold HUD elements such as the minimap and health
    /// globes, which animate whether or not the character moves.
    /// </summary>
    public static double MeanAbsolute(ReadOnlySpan<byte> before, ReadOnlySpan<byte> after, int width, int height, double centralFraction)
    {
        if (before.Length != width * height * 4 || after.Length != before.Length)
        {
            throw new ArgumentException("Both frames must be width × height × 4 bytes.");
        }

        int marginX = (int)(width * (1 - centralFraction) / 2);
        int marginY = (int)(height * (1 - centralFraction) / 2);
        long total = 0;
        long samples = 0;

        for (int y = marginY; y < height - marginY; y++)
        {
            for (int x = marginX; x < width - marginX; x++)
            {
                int offset = (y * width + x) * 4;

                // Alpha is skipped: screen captures leave it constant or undefined.
                for (int channel = 0; channel < 3; channel++)
                {
                    total += Math.Abs(before[offset + channel] - after[offset + channel]);
                }

                samples += 3;
            }
        }

        if (samples == 0)
        {
            return 0;
        }

        return (double)total / samples;
    }
}
