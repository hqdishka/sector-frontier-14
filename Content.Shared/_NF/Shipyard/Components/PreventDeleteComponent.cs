using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._NF.Shipyard.Components
{
    [RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
    public sealed partial class PreventDeleteComponent : Component
    {
        [DataField, AutoNetworkedField]
        public bool Remover = false;
    }
}
