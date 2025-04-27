// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Server.Maps;
using Robust.Shared.Prototypes;

namespace Content.Server.LW.AsteroidSector.Components;
[RegisterComponent]
public sealed partial class AsteroidSectorComponent : Component
{
    [DataField]
    public ProtoId<GameMapPrototype> Station = "Beacon";

    [DataField]
    public bool Enabled = true;
}

