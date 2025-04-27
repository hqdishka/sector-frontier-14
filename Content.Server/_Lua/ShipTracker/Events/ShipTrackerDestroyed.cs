using Content.Server._Lua.ShipTracker.Components;

namespace Content.Server._Lua.ShipTracker.Events;

public sealed class ShipTrackerDestroyed : EntityEventArgs
{
    public EntityUid Ship;
    public ShipTrackerComponent Component;

    public ShipTrackerDestroyed(EntityUid ship, ShipTrackerComponent component)
    {
        Ship = ship;
        Component = component;
    }
}
