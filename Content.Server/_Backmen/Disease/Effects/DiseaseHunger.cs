using Content.Server.Medical;
using Content.Shared.Backmen.Disease;
using Content.Shared.Nutrition.EntitySystems;
using Content.Shared.Nutrition.Components;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Server.Backmen.Disease.Effects;

[UsedImplicitly]
public sealed partial class DiseaseHunger : DiseaseEffect
{
    [DataField("thirstAmount")]
    public float ThirstAmount = -0.01f;
    [DataField("hungerAmount")]
    public float HungerAmount = -0.01f;

    public override object GenerateEvent(Entity<DiseaseCarrierComponent> ent, ProtoId<DiseasePrototype> disease)
    {
        return new DiseaseEffectArgs<DiseaseHunger>(ent, disease, this);
    }
}
public sealed partial class DiseaseEffectSystem
{
    [Dependency] private readonly HungerSystem _hungerSystem = default!;
    [Dependency] private readonly ThirstSystem _thirstSystem = default!;

    private void DiseaseHunger(Entity<DiseaseCarrierComponent> ent, ref DiseaseEffectArgs<DiseaseHunger> args)
    {
        if (args.Handled)
            return;

        args.Handled = true;

        // Apply hunger reduction using the HungerSystem
        if (EntityManager.TryGetComponent(ent.Owner, out HungerComponent? hunger))
        {
            _hungerSystem.ModifyHunger(ent.Owner, args.DiseaseEffect.HungerAmount, hunger);
        }

        // Apply thirst reduction using the ThirstSystem
        if (EntityManager.TryGetComponent(ent.Owner, out ThirstComponent? thirst))
        {
            _thirstSystem.ModifyThirst(ent.Owner, thirst, args.DiseaseEffect.ThirstAmount);
        }
    }
}
