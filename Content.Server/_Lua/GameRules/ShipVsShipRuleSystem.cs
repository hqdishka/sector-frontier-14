// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using Content.Server._Lua.GameRule.Components;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules;
using Content.Server.Chat.Systems;
using Content.Shared.GameTicking.Components;
using Robust.Server.GameObjects;
using Robust.Shared.Configuration;
using Robust.Server.Player;
using Robust.Shared.Map;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Timing;
using Content.Shared.Shuttles.Components;
using Robust.Shared.Player;

public sealed class ShipVsShipRuleSystem : GameRuleSystem<ShipVsShipRuleComponent>
{
    [Dependency] private readonly GameTicker _ticker = default!;
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ChatSystem _chatSystem = default!;

    private TimeSpan _sectorEnableTime;
    private bool _initialAnnouncementPlayed = false;
    private bool _musicPlayed = false;
    private bool _finalAnnouncementPlayed = false;

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void Started(EntityUid uid, ShipVsShipRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
    {
        base.Started(uid, component, gameRule, args);

        _sectorEnableTime = _gameTiming.CurTime + TimeSpan.FromMinutes(15);
        _initialAnnouncementPlayed = false;
        _musicPlayed = false;
        _finalAnnouncementPlayed = false;

        var mapUid = _ticker.DefaultMap;
        var mapEntity = _mapManager.GetMapEntityId(mapUid);

        if (TryComp<FTLDestinationComponent>(mapEntity, out var ftlDestination))
        {
            ftlDestination.Enabled = false;
            Dirty(mapEntity, ftlDestination);
        }

        Timer.Spawn(TimeSpan.FromSeconds(30), () =>
        {
            if (!GameTicker.IsGameRuleActive(uid))
                return;

            _audio.PlayGlobal("/Audio/_Lua/Announcements/attention.ogg", Filter.Broadcast(), true);
            _initialAnnouncementPlayed = true;

            var message = "Всем подготовиться к войне! Координаты сектора скоро будут добавлены во все компьютеры шаттлов. Время на подготовку 15 минут.";
            _chatSystem.DispatchGlobalAnnouncement(
                message: message,
                sender: "Командование",
                colorOverride: Color.Gold
            );
        });

        GenerateShipVsShipContent(mapUid, mapEntity);
    }

    private void GenerateShipVsShipContent(MapId mapId, EntityUid mapEntity)
    {
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ShipVsShipRuleComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var rule, out var gameRule))
        {
            if (!GameTicker.IsGameRuleActive(uid, gameRule))
                continue;

            var timeLeft = _sectorEnableTime - _gameTiming.CurTime;

            if (!_musicPlayed && timeLeft <= TimeSpan.FromMinutes(1))
            {
                _audio.PlayGlobal("/Audio/_Lua/Music/operation.ogg", Filter.Broadcast(), true);
                _musicPlayed = true;

                var message = "Сектор будет открыт для перелетов через 1 минуту!";
                _chatSystem.DispatchGlobalAnnouncement(
                    message: message,
                    sender: "Командование",
                    colorOverride: Color.Gold
                );
            }

            if (!_finalAnnouncementPlayed && timeLeft <= TimeSpan.Zero)
            {
                var mapUid = _ticker.DefaultMap;
                var mapEntity = _mapManager.GetMapEntityId(mapUid);

                if (TryComp<FTLDestinationComponent>(mapEntity, out var ftlDestination))
                {
                    ftlDestination.Enabled = true;
                    Dirty(mapEntity, ftlDestination);
                }

                _audio.PlayGlobal("/Audio/_Lua/Announcements/attention.ogg", Filter.Broadcast(), true);
                _finalAnnouncementPlayed = true;

                var message = "Сектор теперь открыт для перелетов! Всем кораблям - приготовиться к бою!";
                _chatSystem.DispatchGlobalAnnouncement(
                    message: message,
                    sender: "Командование",
                    colorOverride: Color.Gold
                );
            }
        }
    }

    protected override void Ended(EntityUid uid, ShipVsShipRuleComponent component, GameRuleComponent gameRule, GameRuleEndedEvent args)
    {
        base.Ended(uid, component, gameRule, args);

        var mapUid = _ticker.DefaultMap;
        var mapEntity = _mapManager.GetMapEntityId(mapUid);

        if (TryComp<FTLDestinationComponent>(mapEntity, out var ftlDestination))
        {
            ftlDestination.Enabled = true;
            Dirty(mapEntity, ftlDestination);
        }
    }
}
