namespace GDPilot.Output;

public enum MouseButton
{
    Left,
    Right,
}

public sealed class MouseSender
{
    private const int VirtualScreenLeft = 76;
    private const int VirtualScreenTop = 77;
    private const int VirtualScreenWidth = 78;
    private const int VirtualScreenHeight = 79;

    public void MoveRelative(int dx, int dy)
    {
        Send(dx, dy, NativeMethods.MouseEventMove);
    }

    public void MoveAbsolute(int x, int y)
    {
        // Absolute coordinates are normalised to 0..65535 across the whole
        // virtual desktop, so a second monitor to the left shifts the origin.
        int left = NativeMethods.GetSystemMetrics(VirtualScreenLeft);
        int top = NativeMethods.GetSystemMetrics(VirtualScreenTop);
        int width = NativeMethods.GetSystemMetrics(VirtualScreenWidth);
        int height = NativeMethods.GetSystemMetrics(VirtualScreenHeight);

        int normalisedX = (int)Math.Round((x - left) * 65535.0 / (width - 1));
        int normalisedY = (int)Math.Round((y - top) * 65535.0 / (height - 1));

        uint flags = NativeMethods.MouseEventMove | NativeMethods.MouseEventAbsolute | NativeMethods.MouseEventVirtualDesktop;
        Send(normalisedX, normalisedY, flags);
    }

    public void Down(MouseButton button)
    {
        if (button == MouseButton.Left)
        {
            Send(0, 0, NativeMethods.MouseEventLeftDown);
        }
        else
        {
            Send(0, 0, NativeMethods.MouseEventRightDown);
        }
    }

    public void Up(MouseButton button)
    {
        if (button == MouseButton.Left)
        {
            Send(0, 0, NativeMethods.MouseEventLeftUp);
        }
        else
        {
            Send(0, 0, NativeMethods.MouseEventRightUp);
        }
    }

    public void Click(MouseButton button)
    {
        Down(button);
        Up(button);
    }

    public (int X, int Y) CursorPosition()
    {
        NativeMethods.GetCursorPos(out NativeMethods.NativePoint point);
        return (point.X, point.Y);
    }

    private static void Send(int dx, int dy, uint flags)
    {
        NativeMethods.Input input = new();
        input.Type = NativeMethods.InputMouse;
        input.Data.Mouse.Dx = dx;
        input.Data.Mouse.Dy = dy;
        input.Data.Mouse.Flags = flags;

        NativeMethods.SendOne(input);
    }
}
