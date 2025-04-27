// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Server.Maps;
using Robust.Shared.Prototypes;

namespace Content.Server.LW.PirateSector.Components;
[RegisterComponent]
public sealed partial class PirateSectorComponent : Component
{
    [DataField]
    public ProtoId<GameMapPrototype> Station = "Cove";

    [DataField]
    public bool Enabled = true;
}

