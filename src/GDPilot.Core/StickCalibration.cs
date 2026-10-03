namespace GDPilot.Core;

public static class StickCalibration
{
    /// <summary>
    /// Furthest a resting stick may sit from true centre. A worn budget stick rests
    /// well inside this. A mean further out means a thumb was on it at startup, and
    /// accepting it would turn that push into the new idea of rest.
    /// </summary>
    public const double MaximumCentreOffset = 0.3;

    /// <summary>How far any one sample may stray from the mean before the stick counts as moving.</summary>
    public const double MaximumSpread = 0.08;

    /// <summary>
    /// Finds where a stick rests from samples taken while it should be untouched.
    /// Returns false rather than a centre when the samples show it was being held
    /// or moved, so the caller keeps its previous centre.
    /// </summary>
    public static bool TryFindCentre(IReadOnlyList<StickPosition> samples, out StickPosition centre)
    {
        centre = StickPosition.Centre;

        if (samples.Count == 0)
        {
            return false;
        }

        double sumX = 0;
        double sumY = 0;

        foreach (StickPosition sample in samples)
        {
            sumX += sample.X;
            sumY += sample.Y;
        }

        StickPosition mean = new(sumX / samples.Count, sumY / samples.Count);

        if (mean.Magnitude > MaximumCentreOffset)
        {
            return false;
        }

        foreach (StickPosition sample in samples)
        {
            StickPosition offset = new(sample.X - mean.X, sample.Y - mean.Y);

            if (offset.Magnitude > MaximumSpread)
            {
                return false;
            }
        }

        centre = mean;
        return true;
    }
}
