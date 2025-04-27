// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Server.Maps;
using Robust.Shared.Prototypes;

namespace Content.Server.LW.ShipVsShip.Components;
[RegisterComponent]
public sealed partial class ShipVsShipRedComponent : Component
{
    [DataField]
    public ProtoId<GameMapPrototype> Station = "ShipVsShipRedGrid";

    [DataField]
    public bool Enabled = true;
}

