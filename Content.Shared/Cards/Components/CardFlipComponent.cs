using Robust.Shared.Audio;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Cards;

/// <summary>
/// This component allows an entity to be flipped, changing its sprite state.
/// Used for playing cards.
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SharedCardFlipSystem))]
[AutoGenerateComponentState(true)]
public sealed partial class CardFlipComponent : Component
{
    /// <summary>
    /// Is the card currently flipped face-down?
    /// </summary>
    [DataField("flipped"), AutoNetworkedField]
    public bool Flipped { get; set; } = false;

    /// <summary>
    /// The sprite state to use when the card is face-up.
    /// This should be defined in the entity's RSI file.
    /// </summary>
    [DataField("frontState", required: true)]
    public string FrontState { get; private set; } = default!;

    /// <summary>
    /// The sprite state to use when the card is face-down.
    /// This should be defined in the entity's RSI file.
    /// </summary>
    [DataField("backState", required: true)]
    public string BackState { get; private set; } = default!;

    /// <summary>
    /// The sound to play when the card is flipped.
    /// </summary>
    [DataField("flipSound")]
    public SoundSpecifier FlipSound { get; private set; } = new SoundPathSpecifier("/Audio/Items/Cards/card-flip.ogg");
}
