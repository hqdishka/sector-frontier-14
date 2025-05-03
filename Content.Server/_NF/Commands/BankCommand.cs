using System.Linq;
using Content.Server.Administration;
using Content.Server.Preferences.Managers;
using Content.Server._NF.Bank;
using Content.Server.Chat.Managers; //Lua logs
using Content.Server.Administration.Logs; //Lua logs
using Content.Shared.Administration;
using Content.Shared.Database; //Lua logs
using Content.Shared.Preferences;
using Content.Shared._NF.Bank.Components;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Server._NF.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class BankCommand : IConsoleCommand
{
    [Dependency] private readonly IServerPreferencesManager _prefsManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntitySystemManager _entitySystemManager = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IAdminLogManager _adminLogger = default!; //Lua logs
    [Dependency] private readonly IChatManager _chatManager = default!; //Lua logs

    public string Command => "bank";

    public string Description => Loc.GetString($"cmd-{Command}-desc"); // Lua Localization

    public string Help => Loc.GetString($"cmd-{Command}-help"); // Lua Localization

    public void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteError(Loc.GetString("cmd-bank-wrong-args")); // Lua Localization
            return;
        }

        var target = args[0];
        var bankSystem = _entitySystemManager.GetEntitySystem<BankSystem>();

        if (!int.TryParse(args[1], out var amount))
        {
            shell.WriteError(Loc.GetString("cmd-bank-invalid-amount")); // Lua Localization
            return;
        }

        // Try to find player by name first
        ICommonSession? targetSession = null;

        foreach (var session in _playerManager.Sessions)
        {
            if (session.Name.Equals(target, StringComparison.OrdinalIgnoreCase))
            {
                targetSession = session;
                break;
            }
        }

        // If player name not found, try by ID
        if (targetSession == null && Guid.TryParse(target, out var userId))
        {
            targetSession = _playerManager.Sessions.FirstOrDefault(s => s.UserId == userId);
        }

        if (targetSession == null)
        {
            shell.WriteError(Loc.GetString("cmd-bank-player-not-found", ("player", target))); // Lua Localization
            return;
        }

        if (!_prefsManager.TryGetCachedPreferences(targetSession.UserId, out var prefs))
        {
            shell.WriteError(Loc.GetString("cmd-bank-preferences-not-found", ("player", target))); // Lua Localization
            return;
        }

        if (prefs.SelectedCharacter is not HumanoidCharacterProfile profile)
        {
            shell.WriteError(Loc.GetString("cmd-bank-invalid-character", ("player", target))); // Lua Localization
            return;
        }

        var currentBalance = profile.BankBalance;

        // Ensure the player won't have negative balance after withdrawal
        if (amount < 0 && Math.Abs(amount) > currentBalance)
        {
            shell.WriteError(Loc.GetString("cmd-bank-not-enough-money", ("player", target), ("balance", currentBalance), ("removeAmount", Math.Abs(amount)))); // Lua Localization
            return;
        }

        bool success;
        int? newBalance = null;

        // Check if player is currently in-game with an entity
        EntityUid? playerEntity = targetSession.AttachedEntity;

        if (playerEntity != null && _entityManager.HasComponent<BankAccountComponent>(playerEntity.Value))
        {
            // Player is in-game with entity that has bank account - use entity methods which will update the profile
            if (amount > 0)
            {
                success = bankSystem.TryBankDeposit(playerEntity.Value, amount);
                if (success)
                {
                    // Get updated balance after deposit
                    success = bankSystem.TryGetBalance(targetSession, out int updatedBalance);
                    if (success)
                        newBalance = updatedBalance;
                }
            }
            else if (amount < 0)
            {
                success = bankSystem.TryBankWithdraw(playerEntity.Value, Math.Abs(amount));
                if (success)
                {
                    // Get updated balance after withdrawal
                    success = bankSystem.TryGetBalance(targetSession, out int updatedBalance);
                    if (success)
                        newBalance = updatedBalance;
                }
            }
            else
            {
                shell.WriteLine(Loc.GetString("cmd-bank-no-change", ("player", target), ("balance", currentBalance))); // Lua Localization
                return;
            }

            if (success)
                shell.WriteLine(Loc.GetString("cmd-bank-updated")); // Lua Localization
        }
        else
        {
            // Player is not in-game or entity has no bank account - update profile directly
            if (amount > 0)
            {
                success = bankSystem.TryBankDeposit(targetSession, prefs, profile, amount, out newBalance);
            }
            else if (amount < 0)
            {
                success = bankSystem.TryBankWithdraw(targetSession, prefs, profile, Math.Abs(amount), out newBalance);
            }
            else
            {
                shell.WriteLine(Loc.GetString("cmd-bank-no-change", ("player", target), ("balance", currentBalance))); // Lua Localization
                return;
            }
        }

        if (!success || newBalance == null)
        {
            shell.WriteError(Loc.GetString("cmd-bank-failed", ("player", target))); // Lua Localization
            return;
        }

        shell.WriteLine(amount > 0
            ? Loc.GetString("cmd-bank-added", ("amount", amount), ("player", target), ("balance", newBalance.Value)) // Lua Localization
            : Loc.GetString("cmd-bank-removed", ("amount", Math.Abs(amount)), ("player", target), ("balance", newBalance.Value))); // Lua Localization

        //Lua start logs
        var executor = shell.Player?.Name ?? "CONSOLE";
        var absAmount = Math.Abs(amount);
        var actionText = amount > 0 ? Loc.GetString("cmd-bank-log-deposited") : Loc.GetString("cmd-bank-log-withdrew");

        _adminLogger.Add(LogType.BankTransaction, LogImpact.High,
        $"{executor} {actionText} {absAmount} {(amount > 0 ? Loc.GetString("cmd-bank-admin-to") : Loc.GetString("cmd-bank-admin-from"))} {target}. {Loc.GetString("cmd-bank-admin-new-balance", ("balance", newBalance.Value))}");

        _chatManager.DispatchServerAnnouncement(Loc.GetString("cmd-bank-admin-chat", ("executor", executor), ("action", actionText), ("amount", absAmount), ("target", target), ("balance", newBalance.Value))
        );
        //Lua end logs
    }

    public CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
        {
            var options = new List<CompletionOption>();

            // Add all online players as completion options
            foreach (var session in IoCManager.Resolve<IPlayerManager>().Sessions)
            {
                options.Add(new CompletionOption(session.Name, Loc.GetString("cmd-bank-hint-player", ("player", session.Name)))); // Lua Localization
            }

            return CompletionResult.FromOptions(options);
        }

        if (args.Length == 2)
        {
            // For the amount parameter, provide some common values as suggestions
            var amountOptions = new List<CompletionOption>
            {
                new CompletionOption("100", Loc.GetString("cmd-bank-hint-add", ("amount", 100))), // Lua Localization
                new CompletionOption("1000", Loc.GetString("cmd-bank-hint-add", ("amount", 1000))), // Lua Localization
                new CompletionOption("10000", Loc.GetString("cmd-bank-hint-add", ("amount", 10000))), // Lua Localization
                new CompletionOption("-100", Loc.GetString("cmd-bank-hint-remove", ("amount", 100))), // Lua Localization
                new CompletionOption("-1000", Loc.GetString("cmd-bank-hint-remove", ("amount", 1000))) // Lua Localization
            };

            return CompletionResult.FromOptions(amountOptions);
        }

        return CompletionResult.Empty;
    }
}
