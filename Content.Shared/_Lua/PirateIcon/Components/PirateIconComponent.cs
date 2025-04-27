// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Robust.Shared.GameStates;
using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;

namespace Content.Shared.Lua.PirateIcon.Components;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedPirateIconSystem))]
public sealed partial class PirateIconComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<FactionIconPrototype> StatusIcon { get; set; } = "PirateFaction";

    public override bool SessionSpecific => true;

    [DataField]
    public bool IconVisibleToGhost { get; set; }  = true;
}
