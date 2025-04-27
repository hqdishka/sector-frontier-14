namespace Content.Server._Lua.ShipTracker.Rules.EndOnShipDestruction;

[RegisterComponent, Access(typeof(EndOnShipDestructionSystem))]
public sealed partial class EndOnShipDestructionComponent : Component
{
    public EntityUid MainShip = default!;
}
