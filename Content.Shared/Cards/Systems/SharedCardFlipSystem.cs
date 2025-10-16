using Content.Shared.Cards;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Audio;

namespace Content.Shared.Cards;

/// <summary>
/// This system handles the logic for the CardFlipComponent,
/// allowing players to flip cards.
/// </summary>
public sealed class SharedCardFlipSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        // This subscribes to an event that asks for right-click menu options (verbs).
        SubscribeLocalEvent<CardFlipComponent, GetVerbsEvent<AlternativeVerb>>(OnGetVerbs);
    }

    /// <summary>
    /// This method is called when a player right-clicks an entity with a CardFlipComponent.
    /// It adds the "Flip" verb to the context menu.
    /// </summary>
    private void OnGetVerbs(EntityUid uid, CardFlipComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
        // Don't show the verb if the user can't interact with the card.
        if (!args.CanInteract || args.Hands == null)
            return;

        AlternativeVerb verb = new()
        {
            Text = "Flip",
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/refresh.svg.192dpi.png")),
            Act = () =>
            {
                // When the verb is clicked, call our flip logic.
                TryFlip(uid, args.User, component);
            }
        };

        args.Verbs.Add(verb);
    }

    /// <summary>
    /// Performs the actual flip logic.
    /// </summary>
    public void TryFlip(EntityUid uid, EntityUid user, CardFlipComponent? component = null)
    {
        // This is a standard way to ensure we have the component.
        if (!Resolve(uid, ref component))
            return;

        // Invert the flipped state.
        component.Flipped = !component.Flipped;

        // Update the visual appearance of the card.
        UpdateAppearance(uid, component);

        // Play the flip sound for everyone nearby.
        _audio.PlayPvs(component.FlipSound, uid);

        // This tells the server that the component's state has changed and needs to be synced to clients.
        Dirty(uid, component);
    }

    /// <summary>
    /// Updates the sprite of the card based on its Flipped state.
    /// </summary>
    private void UpdateAppearance(EntityUid uid, CardFlipComponent component)
    {
        // This is a check to see if the entity has an AppearanceComponent. If not, we can't change its visuals.
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        // Choose the correct sprite state based on the Flipped property.
        var state = component.Flipped ? component.BackState : component.FrontState;

        // Set the visual state. This is often used for more complex visuals,
        // but here we can just use it to set the base sprite.
        _appearance.SetData(uid, CardFlipVisuals.State, state, appearance);
    }
}


// This enum is used to key the visual data in the AppearanceComponent.
// It's a robust way to ensure we're always setting/getting the same visual property.
public enum CardFlipVisuals : byte
{
    State
}
