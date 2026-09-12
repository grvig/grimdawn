using GDPilot.Core;

namespace GDPilot.Output;

/// <summary>
/// Which of the two key identities the game is expected to read. Older engines
/// often ignore virtual key codes and look only at scan codes, so both are
/// built and the feasibility probe picks the one that works.
/// </summary>
public enum KeyInjectionMode
{
    VirtualKey,
    ScanCode,
}

public sealed class KeySender
{
    private static readonly HashSet<KeyCode> ExtendedKeys = new()
    {
        KeyCode.Left,
        KeyCode.Up,
        KeyCode.Right,
        KeyCode.Down,
        KeyCode.Home,
        KeyCode.End,
        KeyCode.PageUp,
        KeyCode.PageDown,
        KeyCode.Insert,
        KeyCode.Delete,
        KeyCode.RightControl,
        KeyCode.RightAlt,
    };

    private readonly KeyInjectionMode mode;

    public KeySender(KeyInjectionMode mode)
    {
        this.mode = mode;
    }

    public void Down(KeyCode key)
    {
        Send(key, 0);
    }

    public void Up(KeyCode key)
    {
        Send(key, NativeMethods.KeyEventKeyUp);
    }

    private void Send(KeyCode key, uint extraFlags)
    {
        NativeMethods.KeyboardInput keyboard = new();
        keyboard.Flags = extraFlags;

        if (ExtendedKeys.Contains(key))
        {
            keyboard.Flags |= NativeMethods.KeyEventExtendedKey;
        }

        if (mode == KeyInjectionMode.ScanCode)
        {
            // A scan code event carries no virtual key. The driver stack derives
            // one, which is what an engine reading raw scan codes expects to see.
            keyboard.ScanCode = (ushort)NativeMethods.MapVirtualKey((uint)key, NativeMethods.MapVirtualKeyToScanCode);
            keyboard.Flags |= NativeMethods.KeyEventScanCode;
        }
        else
        {
            keyboard.VirtualKey = (ushort)key;
        }

        NativeMethods.Input input = new();
        input.Type = NativeMethods.InputKeyboard;
        input.Data.Keyboard = keyboard;

        NativeMethods.SendOne(input);
    }
}
