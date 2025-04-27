using Content.Shared._Mono.Company;
using Content.Shared.Examine;
using Robust.Shared.Prototypes;

namespace Content.Client._Mono.Company;

/// <summary>
/// Client-side system for displaying company information in examine text.
/// </summary>
public sealed class CompanySystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<Shared._Mono.Company.CompanyComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(EntityUid uid, Shared._Mono.Company.CompanyComponent component, ExaminedEvent args)
    {
        // Try to get the prototype for the company
        if (_prototypeManager.TryIndex<CompanyPrototype>(component.CompanyName, out var prototype))
        {
            // Use the color from the prototype
            args.PushMarkup(Loc.GetString("humanoid-profile-editor-company-label") + $" [color={prototype.Color.ToHex()}]{prototype.Name}[/color]");
        }
        else
        {
            // Fallback for companies without prototypes
            args.PushMarkup(Loc.GetString("humanoid-profile-editor-company-label") + $" [color=yellow]{component.CompanyName}[/color]");
        }
    }
}
