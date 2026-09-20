namespace GDPilot.Vision;

/// <summary>A captured region as 32-bit BGRA pixels, one row after another from the top.</summary>
public sealed record CapturedFrame(byte[] Pixels, int Width, int Height);

public static class ScreenCapture
{
    /// <summary>
    /// Copies a rectangle of the screen. Captured from the screen rather than from
    /// a window's own device context because a game drawing through DirectX leaves
    /// its window context empty, while the composited screen always holds pixels.
    /// </summary>
    public static CapturedFrame Region(int x, int y, int width, int height)
    {
        if (width <= 0 || height <= 0)
        {
            throw new ArgumentException($"Capture region must have positive size, got {width}×{height}.");
        }

        IntPtr screen = NativeGdi.GetDC(IntPtr.Zero);
        IntPtr memory = IntPtr.Zero;
        IntPtr bitmap = IntPtr.Zero;

        try
        {
            memory = NativeGdi.CreateCompatibleDC(screen);
            bitmap = NativeGdi.CreateCompatibleBitmap(screen, width, height);

            if (memory == IntPtr.Zero || bitmap == IntPtr.Zero)
            {
                throw new InvalidOperationException("Could not allocate a capture surface.");
            }

            IntPtr previous = NativeGdi.SelectObject(memory, bitmap);
            bool copied = NativeGdi.BitBlt(memory, 0, 0, width, height, screen, x, y, NativeGdi.SourceCopy | NativeGdi.CaptureBlt);
            NativeGdi.SelectObject(memory, previous);

            if (!copied)
            {
                throw new InvalidOperationException($"Could not capture {width}×{height} at {x},{y}.");
            }

            return ReadPixels(memory, bitmap, width, height);
        }
        finally
        {
            if (bitmap != IntPtr.Zero)
            {
                NativeGdi.DeleteObject(bitmap);
            }

            if (memory != IntPtr.Zero)
            {
                NativeGdi.DeleteDC(memory);
            }

            NativeGdi.ReleaseDC(IntPtr.Zero, screen);
        }
    }

    private static CapturedFrame ReadPixels(IntPtr deviceContext, IntPtr bitmap, int width, int height)
    {
        NativeGdi.BitmapInfo info = new();
        info.Header.Size = (uint)System.Runtime.InteropServices.Marshal.SizeOf<NativeGdi.BitmapInfoHeader>();
        info.Header.Width = width;

        // A negative height asks for top-down rows, matching the order callers expect.
        info.Header.Height = -height;
        info.Header.Planes = 1;
        info.Header.BitCount = 32;
        info.Header.Compression = NativeGdi.BiRgb;

        byte[] pixels = new byte[width * height * 4];
        int lines = NativeGdi.GetDIBits(deviceContext, bitmap, 0, (uint)height, pixels, ref info, NativeGdi.DibRgbColors);

        if (lines != height)
        {
            throw new InvalidOperationException($"Expected {height} captured rows, got {lines}.");
        }

        return new CapturedFrame(pixels, width, height);
    }
}
