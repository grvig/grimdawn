namespace GDPilot.Core;

public static class StickShaping
{
    /// <summary>
    /// Removes resting drift and rescales what is left. The deadzone is radial
    /// rather than per-axis, because a per-axis deadzone snaps shallow diagonals
    /// onto the nearest axis and makes eight-way movement feel sticky.
    /// </summary>
    /// <param name="raw">The stick as the hardware reports it.</param>
    /// <param name="centre">Where this stick actually rests, from calibration.</param>
    /// <param name="deadzone">Radius, measured from the calibrated centre, that reads as rest.</param>
    public static StickPosition ApplyRadialDeadzone(StickPosition raw, StickPosition centre, double deadzone)
    {
        if (deadzone < 0 || deadzone >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(deadzone), deadzone, "Deadzone must be at least 0 and below 1.");
        }

        // A drifted centre shortens the throw on the side it leans towards. Taking
        // the whole offset off the reach keeps full output reachable in every
        // direction, at the cost of saturating slightly early on the far side.
        double reach = 1.0 - centre.Magnitude - deadzone;

        if (reach <= 0)
        {
            throw new ArgumentException($"A centre {centre.Magnitude:F2} from true rest leaves no usable travel beyond a {deadzone:F2} deadzone.");
        }

        double x = raw.X - centre.X;
        double y = raw.Y - centre.Y;
        double magnitude = Math.Sqrt(x * x + y * y);

        if (magnitude <= deadzone)
        {
            return StickPosition.Centre;
        }

        double scaled = (magnitude - deadzone) / reach;

        // Square-gated sticks reach about 1.41 in the corners.
        if (scaled > 1)
        {
            scaled = 1;
        }

        return new StickPosition(x / magnitude * scaled, y / magnitude * scaled);
    }
}
