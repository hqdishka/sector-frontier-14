using System.Numerics;
using Content.Server._Lua.FTLPoints.Systems;
using Content.Shared._Lua.FtlPoints;
using Robust.Shared.Map;

namespace Content.Server._Lua.FTLPoints.Components;

/// <summary>
/// This is used for tracking FTL points and their positioning.
/// </summary>
[RegisterComponent, Access(typeof(FtlPointsSystem), Other = AccessPermissions.Read)]
public sealed partial class StarMapComponent : Component
{
    [ViewVariables] public readonly List<Star> StarMap = new ();
}

