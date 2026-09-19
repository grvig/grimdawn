namespace GDPilot.Integration.Tests;

/// <summary>
/// Every loopback window records all injected input on the machine, so two
/// running at once would each see the other's events. One shared window, with
/// its test classes run one after another, keeps each log attributable.
/// </summary>
[CollectionDefinition(Name)]
public sealed class LoopbackCollection : ICollectionFixture<LoopbackFixture>
{
    public const string Name = "Loopback";
}
