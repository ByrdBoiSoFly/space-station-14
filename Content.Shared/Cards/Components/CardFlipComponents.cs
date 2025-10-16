using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Cards;

[RegisterComponent, NetworkedComponent, Access(typeof(SharedCardFlipSystem))]
[AutoGenerateComponentState(true)]
public sealed partial class CardFlipComponent : Component
{
    // Is the card currently flipped face-down?
    [DataField("flipped"), AutoNetworkedField]
    public bool Flipped { get; set; } = false;

    // The sprite state to use when the card is face-up.
    [DataField("frontState", required: true)]
    public string FrontState { get; private set; } = default!;

    // The sprite state to use when the card is face-down.
    [DataField("backState", required: true)]
    public string BackState { get; private set; } = default!;

    [DataField("flipSound")]
    public SoundSpecifier FlipSound { get; private set; } = new SoundPathSpecifier("/Audio/Item/Cards/card-flip.ogg");
}
