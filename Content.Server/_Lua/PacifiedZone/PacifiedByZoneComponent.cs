// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

namespace Content.Server._NF.PacifiedZone
{
    // Denotes an entity as being pacified by a zone.
    // An entity with PacifiedComponent but not PacifiedByZoneComponent is naturally pacified
    // (e.g. through Pax, or the Pious trait)
    [RegisterComponent]
    public sealed partial class PacifiedByZoneComponent : Component
    {
    }
}
