using Content.Server._NF.Bank;
using Content.Server.Popups; // Lua
using Content.Server.Chat.Managers; // Lua
using Content.Shared._NF.Bank.Components;
using Content.Shared.Access.Components;
using Content.Shared.Chat; // Lua
using Content.Shared.GameTicking;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.PDA;
using Content.Shared.Popups; // Lua
using Content.Shared.Roles;
using Robust.Shared.Player;

namespace Content.Server._Corvax.AutoSalarySystem;

public sealed class AutoSalarySystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly BankSystem _bank = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;

    private static float _currentTime = 3600f;

    [ValidatePrototypeId<DepartmentPrototype>]
    private const string FrontierDepartament = "Frontier"; // Lua Dep<Departament
    [ValidatePrototypeId<DepartmentPrototype>]
    private const string SecurityDepartament = "Security"; // Lua Security
    [ValidatePrototypeId<DepartmentPrototype>]
    private const string TypanDepartament = "OutpostTypan"; // Lua add Typan
    [ValidatePrototypeId<DepartmentPrototype>]
    private const string CentCommDepartament = "CentCom"; // Lua add Centcomm
    [ValidatePrototypeId<DepartmentPrototype>]
    private const string CivilianDepartament = "Civilian"; // Lua add Civilian
    [ValidatePrototypeId<DepartmentPrototype>]
    private const string MedicalDepartament = "Medical"; // Lua add Medical

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RoundRestartCleanupEvent>(OnRoundRestart);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        _currentTime -= frameTime;

        if (_currentTime <= 0)
        {
            _currentTime = 3600f;
            ProcessSalary();
        }
    }

    private void OnRoundRestart(RoundRestartCleanupEvent args)
    {
        _currentTime = 3600f;
    }

    private void ProcessSalary()
    {
        var currentTime = EntityQueryEnumerator<HumanoidAppearanceComponent, BankAccountComponent, ActorComponent>();
        while (currentTime.MoveNext(out var uid, out _, out _, out _))
        {
            if (GetDepartment(uid, out var job))
            {
                Logger.Info($"Salary check: {ToPrettyString(uid)} as {job}");
                int salary = GetSalary(job);
                if (_bank.TryBankDeposit(uid, salary))
                {
                    NotifySalaryReceived(uid, salary);  // Lua
                }
            }
        }
    }
    // Lua start
    private void NotifySalaryReceived(EntityUid uid, int salary)
    {
        if (!TryComp(uid, out BankAccountComponent? bank))
            return;

        if (!TryComp(uid, out ActorComponent? actor))
            return;

        var changeAmount = $"+{salary}";
        var message = Loc.GetString(
            "bank-program-change-balance-notification",
            ("balance", bank.Balance),
            ("change", changeAmount),
            ("currencySymbol", "$")
        );

        _popup.PopupEntity(message, uid, Filter.Entities(uid), true, PopupType.Small);

        _chatManager.ChatMessageToOne(
            ChatChannel.Notifications,
            message,
            message,
            EntityUid.Invalid,
            false,
            actor.PlayerSession.Channel
        );
    }
    // Lua end

    private int GetSalary(string key) => key switch
    {
        //Security
        var s when s == Loc.GetString("job-name-sheriff") => 80000,
        var s when s == Loc.GetString("job-name-bailiff") => 65000,
        var s when s == Loc.GetString("job-name-senior-officer") => 60500,
        var s when s == Loc.GetString("job-name-nf-detective") => 40500,
        var s when s == Loc.GetString("job-name-brigmedic") => 45000,
        var s when s == Loc.GetString("job-name-deputy") => 40000,
        var s when s == Loc.GetString("job-name-cadet-nf") => 35000,
        //Frontier
        var s when s == Loc.GetString("job-name-security-guard") => 40500,
        var s when s == Loc.GetString("job-name-mail-carrier") => 30000,
        var s when s == Loc.GetString("job-name-janitor") => 25000,
        var s when s == Loc.GetString("job-name-valet") => 25000,
        var s when s == Loc.GetString("job-name-stc") => 65000,
        var s when s == Loc.GetString("job-name-sr") => 80000,
        var s when s == Loc.GetString("job-name-pal") => 35000,
        var s when s == Loc.GetString("job-name-doc") => 80000,
        //Civilian
        var s when s == Loc.GetString("job-name-contractor") => 6000,
        var s when s == Loc.GetString("job-name-chaplain") => 25500,
        var s when s == Loc.GetString("job-name-pilot") => 7100,
        //Typan
        var s when s == Loc.GetString("job-name-typan-atmos-tech") => 35000,
        var s when s == Loc.GetString("job-name-typan-botanist") => 25000,
        var s when s == Loc.GetString("job-name-typan-cargotech") => 35000,
        var s when s == Loc.GetString("job-name-typan-chef") => 25000,
        var s when s == Loc.GetString("job-name-typan-medic") => 35000,
        var s when s == Loc.GetString("job-name-typan-researcher") => 25000,
        var s when s == Loc.GetString("job-name-typan-rd") => 45000,
        var s when s == Loc.GetString("job-name-typan-science") => 35000,
        var s when s == Loc.GetString("job-name-typan-telecommunications-officer") => 65000,
        //CentralCommand
        var s when s == Loc.GetString("job-name-centcomblueshield") => 205000,
        var s when s == Loc.GetString("job-name-centcomcargo") => 100000,
        var s when s == Loc.GetString("job-name-centcomoffBK") => 140000,
        var s when s == Loc.GetString("job-name-centcomoso") => 327000,
        var s when s == Loc.GetString("job-name-centcomassistant") => 93000,
        var s when s == Loc.GetString("job-name-centcomsecofficer") => 110000,
        var s when s == Loc.GetString("job-name-centcomoper") => 150000,
        //Medical
        var s when s == Loc.GetString("job-name-chemist") => 45000,
        var s when s == Loc.GetString("job-name-paramedic") => 40000,
        var s when s == Loc.GetString("job-name-doctor") => 70000,
        _ => throw new KeyNotFoundException()
    };


    // Lua start
    private static readonly HashSet<string> AllowedDepartments = new()
    {
        FrontierDepartament,
        SecurityDepartament,
        TypanDepartament,
        CentCommDepartament,
        CivilianDepartament,
        MedicalDepartament
    };
    // Lua end

    private bool GetDepartment(EntityUid uid, out string job)
    {
        job = string.Empty;
        var idCard = GetIdCard(uid);

        if (idCard is null)
            return false;

        foreach (var departmentProtoId in idCard.JobDepartments)
        {
            if (AllowedDepartments.Contains(departmentProtoId)) // Lua replace || on HashSet
            {
                job = idCard.LocalizedJobTitle ?? string.Empty;
                return true;
            }
        }
        return false;
    }

    private IdCardComponent? GetIdCard(EntityUid uid)
    {
        if (!_inventory.TryGetSlotEntity(uid, "id", out var idUid))
            return null;

        if (EntityManager.TryGetComponent(idUid, out PdaComponent? pda) && pda.ContainedId != null)
        {
            return TryComp<IdCardComponent>(pda.ContainedId, out var idComp) ? idComp : null;
        }
        return EntityManager.TryGetComponent(idUid, out IdCardComponent? id) ? id : null;
    }
}
