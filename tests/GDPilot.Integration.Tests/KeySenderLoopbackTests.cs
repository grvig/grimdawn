using GDPilot.Core;
using GDPilot.Output;

namespace GDPilot.Integration.Tests;

[Collection(LoopbackCollection.Name)]
public sealed class KeySenderLoopbackTests
{
    private readonly LoopbackFixture loopback;

    public KeySenderLoopbackTests(LoopbackFixture loopback)
    {
        this.loopback = loopback;
    }

    [Fact]
    public void VirtualKeyPressReachesTheOperatingSystem()
    {
        KeySender sender = new(KeyInjectionMode.VirtualKey);
        int before = loopback.LineCount();

        sender.Down(KeyCode.W);
        sender.Up(KeyCode.W);

        List<string> recorded = loopback.WaitForNew(before, lines => lines.Count >= 2);
        Assert.StartsWith("key down vk=57", recorded[0]);
        Assert.StartsWith("key up vk=57", recorded[1]);
    }

    [Fact]
    public void ScanCodePressCarriesTheHardwareScanCode()
    {
        KeySender sender = new(KeyInjectionMode.ScanCode);
        int before = loopback.LineCount();

        sender.Down(KeyCode.W);
        sender.Up(KeyCode.W);

        // W sits at scan code 0x11 on every standard keyboard layout.
        List<string> recorded = loopback.WaitForNew(before, lines => lines.Count >= 2);
        Assert.StartsWith("key down", recorded[0]);
        Assert.EndsWith("scan=11", recorded[0]);
        Assert.StartsWith("key up", recorded[1]);
        Assert.EndsWith("scan=11", recorded[1]);
    }
}
