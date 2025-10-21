using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.List;
using Robust.Shared.Prototypes;
using System.Collections.Generic;

namespace Content.Shared.PlayingCards
{
    /// <summary>
    /// Component for entities that can spawn random playing cards when used.
    /// </summary>
    [RegisterComponent]
    public sealed partial class RandomCardSpawnerComponent : Component
    {
        /// <summary>
        /// List of entity prototypes (card IDs) that can be spawned from this deck.
        /// </summary>
        [DataField("prototypes", customTypeSerializer: typeof(PrototypeIdListSerializer<EntityPrototype>))]
        public List<string> Prototypes = new();

        /// <summary>
        /// If true, when the second-to-last card is drawn, the deck entity itself will be replaced by the last remaining card.
        /// </summary>
        [DataField("lastPrototype")]
        public bool LastPrototype = false;

        /// <summary>
        /// The currently shuffled list of card prototypes. This list is modified as cards are drawn.
        /// </summary>
        [DataField("shuffledPrototypes")]
        public List<string> ShuffledPrototypes = new();
    }
}