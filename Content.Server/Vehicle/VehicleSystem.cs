using Content.Server._Mono.Radar;
using Content.Shared.Buckle.Components;
using Content.Shared.Vehicle;
using Content.Shared.Vehicle.Components;

namespace Content.Server.Vehicle;

public sealed class VehicleSystem : SharedVehicleSystem
{
    protected override void OnStrapped(EntityUid uid, VehicleComponent component, ref StrappedEvent args)
    {
        base.OnStrapped(uid, component, ref args);

        var blip = EnsureComp<RadarBlipComponent>(uid);
        blip.RadarColor = Color.Cyan;
        blip.Scale = 0.5f;
        blip.VisibleFromOtherGrids = true;
    }

    protected override void OnUnstrapped(EntityUid uid, VehicleComponent component, ref UnstrappedEvent args)
    {
        RemComp<RadarBlipComponent>(uid);

        base.OnUnstrapped(uid, component, ref args);
    }
}
