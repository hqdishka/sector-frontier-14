// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

namespace Content.Server._Lua.ShipRename;

/// <summary>
/// This is used for tracking renaming ships
/// </summary>
[RegisterComponent]
public sealed partial class ShipRenameComponent : Component
{
    public EntityUid? GridId { set; get; }
}
