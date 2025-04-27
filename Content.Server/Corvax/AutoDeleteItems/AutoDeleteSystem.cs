using Content.Server.Construction.Completions;
using Content.Shared.Humanoid;
using Content.Shared.Mobs.Systems;
using Robust.Shared.Timing;
using Robust.Shared.Player;
using Content.Server._NF.CryoSleep;
using Content.Shared.Lua.CLVar;
using Robust.Shared.Configuration;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;

namespace Content.Server.Corvax.AutoDeleteItems;

public sealed class AutoDeleteSystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly MobStateSystem _mobStateSystem = default!;

    private bool _autodeleteEnabled = true;

    public override void Initialize()
    {
        base.Initialize();
        _cfg.OnValueChanged(CLVars.AutoDelteEnabled, OnAutoDelteEnabledChanged, true);
    }

    private void OnAutoDelteEnabledChanged(bool value)
    {
        _autodeleteEnabled = value;
        if (!_autodeleteEnabled)
        { }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<AutoDeleteComponent>();
        while (query.MoveNext(out var uid, out var autoDeleteComponent))
        {
            if (autoDeleteComponent.NextTimeToCheck > _gameTiming.CurTime)
                continue;

            autoDeleteComponent.IsCryoNear = false;
            autoDeleteComponent.IsHumanoidNear = false;

            foreach (var iterator in _lookup.GetEntitiesInRange(uid, autoDeleteComponent.DistanceToCheck))
            {
                if (TryComp<CryoSleepComponent>(iterator, out var cryoComponent) && cryoComponent.IsCryo)
                {
                    autoDeleteComponent.IsCryoNear = true;
                    break; // Cryo имеет приоритет, дальнейшие проверки не нужны
                }

                if (TryComp<HumanoidAppearanceComponent>(iterator, out var humanoidComponent) && iterator != uid)
                {
                    // Проверяем, мёртв ли персонаж
                    if (TryComp<MobStateComponent>(iterator, out var mobState) &&
                    (_mobStateSystem.IsDead(iterator, mobState) || _mobStateSystem.IsCritical(iterator, mobState)))
                        continue;

                    autoDeleteComponent.IsHumanoidNear = true;
                }
            }

            // Если есть Cryo, отключаем очередь на удаление в любом случае
            if (autoDeleteComponent.IsCryoNear)
            {
                autoDeleteComponent.ReadyToDelete = false;
                autoDeleteComponent.NextTimeToCheck = _gameTiming.CurTime + autoDeleteComponent.DelayToCheck;
                continue;
            }

            // Удаление, если Humanoid не рядом
            if (!autoDeleteComponent.IsHumanoidNear)
            {
                if (autoDeleteComponent.ReadyToDelete && autoDeleteComponent.NextTimeToDelete < _gameTiming.CurTime)
                {
                    EntityManager.DeleteEntity(uid);
                    continue;
                }

                if (!autoDeleteComponent.ReadyToDelete)
                {
                    autoDeleteComponent.NextTimeToDelete = _gameTiming.CurTime + autoDeleteComponent.DelayToDelete;
                    autoDeleteComponent.ReadyToDelete = true;
                }
            }
            else
            {
                // Сбрасываем флаг удаления, если есть Humanoid
                if (autoDeleteComponent.ReadyToDelete)
                    autoDeleteComponent.ReadyToDelete = false;

                autoDeleteComponent.NextTimeToDelete = _gameTiming.CurTime + autoDeleteComponent.DelayToDelete;
            }

            // Обновляем время следующей проверки
            autoDeleteComponent.NextTimeToCheck = _gameTiming.CurTime + autoDeleteComponent.DelayToCheck;
        }
    }
}
