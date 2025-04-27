// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

namespace Content.Server._Lua.NoShuttleFTL;

[RegisterComponent]
public sealed partial class NoShuttleFTLComponent : Component
{
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool CantFTL = true;
}
