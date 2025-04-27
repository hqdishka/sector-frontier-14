// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Shared.Lua.PirateIcon.Components;
using Content.Shared.Ghost;
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;

namespace Content.Client.LW.PirateIcon;

public sealed class PirateIconSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PirateIconComponent, GetStatusIconsEvent>(GetPirateIcon);
    }

    private void GetPirateIcon(Entity<PirateIconComponent> ent, ref GetStatusIconsEvent args)
    {
        if (HasComp<PirateIconComponent>(ent))
            return;

        if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }

    private bool CanDisplayIcon(EntityUid? uid, bool visibleToGhost)
    {
        if (HasComp<PirateIconComponent>(uid))
            return true;

        if (visibleToGhost && HasComp<GhostComponent>(uid))
            return true;

        return HasComp<ShowPirIconsComponent>(uid);
    }

}
