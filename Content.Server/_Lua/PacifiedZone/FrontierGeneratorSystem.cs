// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Robust.Shared.Timing;
using Content.Shared.Alert;
using Content.Shared.Humanoid;
using Content.Shared.Mind;

namespace Content.Server._NF.FrontierZone
{
    public sealed class FrontierZoneGeneratorSystem : EntitySystem
    {
        [Dependency] private readonly EntityLookupSystem _lookup = default!;
        [Dependency] private readonly IGameTiming _gameTiming = default!;
        [Dependency] private readonly SharedMindSystem _mindSystem = default!;
        [Dependency] private readonly AlertsSystem _alerts = default!;

        private const string Alert = "Frontier";

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<FrontierZoneGeneratorComponent, ComponentInit>(OnComponentInit);
            SubscribeLocalEvent<FrontierZoneGeneratorComponent, ComponentShutdown>(OnComponentShutdown);
        }

        private void OnComponentInit(EntityUid uid, FrontierZoneGeneratorComponent component, ComponentInit args)
        {
            foreach (var humanoidUid in _lookup.GetEntitiesInRange<HumanoidAppearanceComponent>(Transform(uid).Coordinates, component.Radius))
            {
                if (!_mindSystem.TryGetMind(humanoidUid, out var mindId, out var _))
                    continue;

                EnableAlert(humanoidUid);
                AddComp<FrontierZoneComponent>(humanoidUid);
                component.TrackedEntities.Add(humanoidUid);
            }

            component.NextUpdate = _gameTiming.CurTime + component.UpdateInterval;
        }

        private void OnComponentShutdown(EntityUid uid, FrontierZoneGeneratorComponent component, ComponentShutdown args)
        {
            foreach (var entity in component.TrackedEntities)
            {
                RemComp<FrontierZoneComponent>(entity);
                DisableAlert(entity);
            }
        }

        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            var genQuery = AllEntityQuery<FrontierZoneGeneratorComponent>();
            while (genQuery.MoveNext(out var genUid, out var component))
            {
                List<EntityUid> newEntities = new List<EntityUid>();
                if (_gameTiming.CurTime < component.NextUpdate)
                    continue;

                var query = _lookup.GetEntitiesInRange<HumanoidAppearanceComponent>(Transform(genUid).Coordinates, component.Radius);
                foreach (var humanoidUid in query)
                {
                    if (!_mindSystem.TryGetMind(humanoidUid, out var mindId, out var mind))
                        continue;

                    if (component.TrackedEntities.Contains(humanoidUid))
                    {
                        newEntities.Add(humanoidUid);
                        component.TrackedEntities.Remove(humanoidUid);
                    }
                    else
                    {
                        EnableAlert(humanoidUid);
                        AddComp<FrontierZoneComponent>(humanoidUid);
                        newEntities.Add(humanoidUid);
                    }
                }

                foreach (var humanoid_net_uid in component.TrackedEntities)
                {
                    RemComp<FrontierZoneComponent>(humanoid_net_uid);
                    DisableAlert(humanoid_net_uid);
                }

                component.TrackedEntities = newEntities;
                component.NextUpdate = _gameTiming.CurTime + component.UpdateInterval;
            }
        }

        private void EnableAlert(EntityUid entity)
        {
            _alerts.ShowAlert(entity, Alert);
        }

        private void DisableAlert(EntityUid entity)
        {
            _alerts.ClearAlert(entity, Alert);
        }
    }
}
