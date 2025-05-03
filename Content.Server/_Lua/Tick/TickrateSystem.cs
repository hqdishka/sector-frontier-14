// LuaWorld - This file is licensed under AGPLv3
// Copyright (c) 2025 LuaWorld
// See AGPLv3.txt for details.

using System.Linq;
using JetBrains.Annotations;
using Content.Shared.Lua.CLVar;
using Robust.Server.Player;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Player;

namespace Content.Server._Lua.Tick
{
    [UsedImplicitly]
    public sealed class TickrateSystem : EntitySystem
    {
        [Dependency] private readonly IConfigurationManager _cfg = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;

        public override void Initialize()
        {
            base.Initialize();

            _cfg.OnValueChanged(CLVars.NetDynamicTick, dynamicEnabled =>
            {
                if (dynamicEnabled)
                    UpdateTickrate();
            }, true);

            _playerManager.PlayerStatusChanged += OnPlayerStatusChanged;
        }

        private void OnPlayerStatusChanged(object? sender, SessionStatusEventArgs args)
        {
            if (!_cfg.GetCVar(CLVars.NetDynamicTick))
                return;

            if (args.NewStatus == SessionStatus.Connected || args.NewStatus == SessionStatus.Disconnected)
                UpdateTickrate();
        }

        private void UpdateTickrate()
        {
            var inGame = _playerManager.Sessions
                .Count(s => s.Status == SessionStatus.InGame);

            var newRate = inGame switch
            {
                <= 10 => 60,
                <= 15 => 50,
                <= 20 => 40,
                <= 30 => 30,
                _ => 26
            };

            _cfg.SetCVar(CVars.NetTickrate, newRate);
        }
    }
}

// Experimental function
