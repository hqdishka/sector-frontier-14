using Content.Server._Mono.Radar;
using Content.Shared.Buckle.Components;
using Content.Shared._Goobstation.Vehicles; // Frontier: migrate under _Goobstation

namespace Content.Server._Goobstation.Vehicles; // Frontier: migrate under _Goobstation

public sealed class VehicleSystem : SharedVehicleSystem
{
    protected override void OnStrapped(Entity<VehicleComponent> ent, ref StrappedEvent args)
    {
        base.OnStrapped(ent, ref args);

        var blip = EnsureComp<RadarBlipComponent>(ent);
        blip.RadarColor = Color.Cyan;
        blip.Scale = 0.5f;
        blip.VisibleFromOtherGrids = true;
    }

    protected override void OnUnstrapped(Entity<VehicleComponent> ent, ref UnstrappedEvent args)
    {
        RemComp<RadarBlipComponent>(ent);

        base.OnUnstrapped(ent, ref args);
    }
}
