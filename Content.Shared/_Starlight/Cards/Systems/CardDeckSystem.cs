using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Robust.Shared.Random;
using System.Linq; // For .Shuffle()

namespace Content.Shared.PlayingCards
{
    /// <summary>
    /// System for handling the logic of RandomCardSpawnerComponent,
    /// including shuffling, drawing cards, and transforming the deck into the last card.
    /// </summary>
    public sealed class CardDeckSystem : EntitySystem
    {
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly SharedHandsSystem _handsSystem = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<RandomCardSpawnerComponent, ComponentInit>(OnInit);
            SubscribeLocalEvent<RandomCardSpawnerComponent, UseInHandEvent>(OnUseInHand);
        }

        private void OnInit(EntityUid uid, RandomCardSpawnerComponent component, ComponentInit args)
        {
            // Copy the initial prototypes to the shuffled list and shuffle them
            component.ShuffledPrototypes.Clear();
            component.ShuffledPrototypes.AddRange(component.Prototypes);
            _random.Shuffle(component.ShuffledPrototypes);
        }

        private void OnUseInHand(EntityUid uid, RandomCardSpawnerComponent component, UseInHandEvent args)
        {
            // If the event is already handled, or the user is not a player, do nothing.
            if (args.Handled)
                return;

            // If there are no cards left in the deck, we can't draw anything.
            if (component.ShuffledPrototypes.Count == 0)
            {
                // If the deck is already empty, and LastPrototype is true, it means it should have transformed already.
                // In this edge case, we just delete it if it somehow persists.
                if (component.LastPrototype)
                {
                    _entityManager.DeleteEntity(uid);
                }
                args.Handled = true;
                return;
            }

            // Get the prototype ID of the card to be drawn (the first one in the shuffled list)
            var cardPrototypeId = component.ShuffledPrototypes[0];
            // Remove it from the deck's internal list so it cannot be drawn again
            component.ShuffledPrototypes.RemoveAt(0);

            // Spawn the card at the deck's current location
            var spawnedCard = _entityManager.SpawnEntity(cardPrototypeId, _entityManager.GetComponent<TransformComponent>(uid).Coordinates);

            // Try to place the newly spawned card into the user's active hand
            if (_entityManager.TryGetComponent<HandsComponent>(args.User, out var hands))
            {
                _handsSystem.TryPickup(args.User, spawnedCard, hands.ActiveHandId!);
            }

            // Check if this draw was the second-to-last card, triggering the deck to transform into the final card.
            if (component.LastPrototype && component.ShuffledPrototypes.Count == 1)
            {
                var finalCardPrototype = component.ShuffledPrototypes[0]; // The last remaining card
                _entityManager.SpawnEntity(finalCardPrototype, _entityManager.GetComponent<TransformComponent>(uid).Coordinates);
                _entityManager.DeleteEntity(uid); // Delete the deck entity, as it has transformed
            }
            else if (component.ShuffledPrototypes.Count == 0)
            {
                // If the deck is now completely empty (and LastPrototype wasn't triggered or was false), delete it.
                _entityManager.DeleteEntity(uid);
            }

            args.Handled = true;
        }
    }
}