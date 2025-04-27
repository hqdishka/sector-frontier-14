using Robust.Shared.Serialization;

namespace Content.Shared._Lua.ShipTracker;

public abstract class SharedShipTrackerSystem : EntitySystem
{
    [Serializable, NetSerializable]
    public enum ShieldGeneratorVisuals : byte
    {
        State
    }
}
