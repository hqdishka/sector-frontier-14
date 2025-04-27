// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Shared.Ghost;
using Content.Shared.Lua.PirateIcon.Components;
using Robust.Shared.GameStates;
using Robust.Shared.Player;

namespace Content.Shared.Lua.PirateIcon;

public sealed class SharedPirateIconSystem : EntitySystem
{

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PirateIconComponent, ComponentGetStateAttemptEvent>(OnPirCompGetStateAttempt);
        SubscribeLocalEvent<PirateIconComponent, ComponentStartup>(DirtyPirComps);
        SubscribeLocalEvent<ShowPirIconsComponent, ComponentStartup>(DirtyPirComps);
    }

    private void OnPirCompGetStateAttempt(EntityUid uid, PirateIconComponent comp, ref ComponentGetStateAttemptEvent args)
    {
        args.Cancelled = !CanGetState(args.Player, comp.IconVisibleToGhost);
    }

    private bool CanGetState(ICommonSession? player, bool visibleToGhosts)
    {
        if (player is null)
            return true;

        var uid = player.AttachedEntity;

        if (HasComp<PirateIconComponent>(uid))
            return true;

        if (visibleToGhosts && HasComp<GhostComponent>(uid))
            return true;

        return HasComp<ShowPirIconsComponent>(uid);
    }

    private void DirtyPirComps<T>(EntityUid someUid, T someComp, ComponentStartup ev)
    {
        var revComps = AllEntityQuery<PirateIconComponent>();
        while (revComps.MoveNext(out var uid, out var comp))
        {
            Dirty(uid, comp);
        }
    }
}
