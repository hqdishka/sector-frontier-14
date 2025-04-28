// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Robust.Shared.Serialization;

namespace Content.Shared._Lua.CryoTimer;


[Serializable, NetSerializable]
public sealed class CryoReturnTimerEvent(TimeSpan? time) : EntityEventArgs
{
    public readonly TimeSpan? Time = time;
}
