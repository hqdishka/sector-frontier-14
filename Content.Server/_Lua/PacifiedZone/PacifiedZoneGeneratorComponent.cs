// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Robust.Shared.Prototypes;
using Content.Shared.Roles;
using Robust.Shared.Audio; // Frontier

namespace Content.Server._NF.PacifiedZone
{
    [RegisterComponent]
    public sealed partial class PacifiedZoneGeneratorComponent : Component
    {
        /// <summary>
        ///     List of entities that have been notified when entering the zone.
        /// </summary>
        public HashSet<EntityUid> NotifiedEntities { get; set; } = new();

        [ViewVariables]
        public List<EntityUid> TrackedEntities = new();

        [ViewVariables]
        public TimeSpan NextUpdate;

        /// <summary>
        /// The interval at which this component updates.
        /// </summary>
        [DataField]
        public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

        [DataField]
        public int Radius = 5;

        [DataField]
        public List<ProtoId<JobPrototype>> ImmuneRoles = new();

        [DataField]
        public bool KillHostileMobs = false;

        /// <summary>
        /// The sound made when a hostile mob is killed when entering a protected grid.
        /// </summary>
        [DataField]
        public SoundSpecifier HostileMobKillSound = new SoundPathSpecifier("/Audio/Effects/holy.ogg");
        // End Frontier
    }
}
