using System.Runtime.InteropServices;
using static GDPilot.Loopback.NativeHooks;

namespace GDPilot.Loopback;

/// <summary>
/// Records injected input seen by global low-level hooks, then swallows it so a
/// test run never types into whatever window happens to have focus. Real input
/// from the person at the machine is neither logged nor blocked.
/// </summary>
internal sealed class HookRecorder : IDisposable
{
    private const uint KeyInjectedFlag = 0x10;
    private const uint KeyUpFlag = 0x80;
    private const uint MouseInjectedFlag = 0x01;

    private static readonly Dictionary<long, string> MouseMessages = new()
    {
        [0x0200] = "move",
        [0x0201] = "leftdown",
        [0x0202] = "leftup",
        [0x0204] = "rightdown",
        [0x0205] = "rightup",
    };

    private readonly StreamWriter log;

    // Held in fields because the native side keeps only a raw pointer, and a
    // collected delegate crashes the process on the next event.
    private readonly HookProcedure keyboardProcedure;
    private readonly HookProcedure mouseProcedure;
    private readonly IntPtr keyboardHook;
    private readonly IntPtr mouseHook;

    internal HookRecorder(StreamWriter log)
    {
        this.log = log;
        keyboardProcedure = OnKeyboard;
        mouseProcedure = OnMouse;
        keyboardHook = Install(KeyboardLowLevel, keyboardProcedure);
        mouseHook = Install(MouseLowLevel, mouseProcedure);
    }

    public void Dispose()
    {
        UnhookWindowsHookEx(keyboardHook);
        UnhookWindowsHookEx(mouseHook);
    }

    private IntPtr OnMouse(int code, IntPtr message, IntPtr data)
    {
        MouseData mouse = Marshal.PtrToStructure<MouseData>(data);

        if (code < 0 || (mouse.Flags & MouseInjectedFlag) == 0)
        {
            return CallNextHookEx(IntPtr.Zero, code, message, data);
        }

        if (!MouseMessages.TryGetValue(message.ToInt64(), out string? name))
        {
            name = $"message{message.ToInt64():X}";
        }

        // Swallowing a move leaves the real cursor where it was, so the logged
        // point is where the event would have put it.
        log.WriteLine($"mouse {name} x={mouse.X} y={mouse.Y}");
        return 1;
    }

    private static IntPtr Install(int hookType, HookProcedure procedure)
    {
        IntPtr hook = SetWindowsHookEx(hookType, procedure, GetModuleHandle(null), 0);

        if (hook == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Hook {hookType} failed, error {Marshal.GetLastWin32Error()}.");
        }

        return hook;
    }

    private IntPtr OnKeyboard(int code, IntPtr message, IntPtr data)
    {
        KeyboardData key = Marshal.PtrToStructure<KeyboardData>(data);

        if (code < 0 || (key.Flags & KeyInjectedFlag) == 0)
        {
            return CallNextHookEx(IntPtr.Zero, code, message, data);
        }

        string direction = "down";
        if ((key.Flags & KeyUpFlag) != 0)
        {
            direction = "up";
        }

        log.WriteLine($"key {direction} vk={key.VirtualKey:X2} scan={key.ScanCode:X2}");
        return 1;
    }
}
