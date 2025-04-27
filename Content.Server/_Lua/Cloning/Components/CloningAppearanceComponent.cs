// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Shared.Roles;
using Robust.Shared.Prototypes;

namespace Content.Server._Lua.Cloning.Components;

[RegisterComponent]
public sealed partial class CloningAppearanceComponent : Component
{
    [DataField("components")]
    [AlwaysPushInheritance]
    public ComponentRegistry Components { get; private set; } = new();

    [DataField("gear")]
    public ProtoId<StartingGearPrototype>? Gear;
}
