// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Server.Spawners.Components;

[RegisterComponent]
public sealed partial class SingleSpawnerComponent : Component
{
    /// <summary>
    /// Прототипы для обычного шанса.
    /// </summary>
    [DataField]
    public List<EntProtoId> CommonPrototypes = new();

    /// <summary>
    /// Прототипы для редкого шанса.
    /// </summary>
    [DataField]
    public List<EntProtoId> RarePrototypes = new();

    /// <summary>
    /// Прототипы для суперредкого шанса.
    /// </summary>
    [DataField]
    public List<EntProtoId> SuperRarePrototypes = new();

    /// <summary>
    /// Шанс для обычного спауна.
    /// </summary>
    [DataField]
    public float CommonChance = 1.0f;

    /// <summary>
    /// Шанс для редкого спауна.
    /// </summary>
    [DataField]
    public float RareChance = 0.1f;

    /// <summary>
    /// Шанс для суперредкого спауна.
    /// </summary>
    [DataField]
    public float SuperRareChance = 0.01f;

    /// <summary>
    /// Текущий созданный объект.
    /// </summary>
    public EntityUid? SpawnedEntity;
}
