namespace GDPilot.Core;

public enum ProbeOutcome
{
    Pass,
    Fail,
    Inconclusive,
}

public static class ProbeVerdict
{
    /// <summary>
    /// Judges a movement capture against an idle capture of the same length. A
    /// fixed threshold cannot work on its own: rain, fire and idle animation can
    /// outscore a short walk in a quiet area, so the idle score sets the bar.
    /// </summary>
    public static ProbeOutcome Classify(double movedScore, double idleScore)
    {
        double passLine = Math.Max(idleScore * 3.0, idleScore + 4.0);
        double failLine = idleScore * 1.5 + 1.0;

        if (movedScore >= passLine)
        {
            return ProbeOutcome.Pass;
        }

        if (movedScore <= failLine)
        {
            return ProbeOutcome.Fail;
        }

        return ProbeOutcome.Inconclusive;
    }
}
