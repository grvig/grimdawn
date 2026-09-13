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

    private readonly StreamWriter log;

    // Held in fields because the native side keeps only a raw pointer, and a
    // collected delegate crashes the process on the next event.
    private readonly HookProcedure keyboardProcedure;
    private readonly IntPtr keyboardHook;

    internal HookRecorder(StreamWriter log)
    {
        this.log = log;
        keyboardProcedure = OnKeyboard;
        keyboardHook = Install(KeyboardLowLevel, keyboardProcedure);
    }

    public void Dispose()
    {
        UnhookWindowsHookEx(keyboardHook);
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
