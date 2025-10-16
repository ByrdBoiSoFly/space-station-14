using Content.Shared.Cards;
using Content.Shared.Verbs;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Shared.Utility;
using Robust.Shared.Prototypes;
using Content.Shared.Interaction;


namespace Content.Shared.Cards;

public sealed class SharedCardFlipSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CardFlipComponent, GetVerbsEvent<AlternativeVerb>>(OnGetVerbs);
    }

    /// This method is called when a player right-clicks an entity with a CardFlipComponent.
    /// It adds the "Flip" verb to the context menu.
    private void OnGetVerbs(EntityUid uid, CardFlipComponent component, GetVerbsEvent<AlternativeVerb> args)
    {
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

    public void TryFlip(EntityUid uid, EntityUid user, CardFlipComponent? component = null)
    {
        if (!Resolve(uid, ref component))
            return;
        // Invert the flipped state.
        component.Flipped = !component.Flipped;
        UpdateAppearance(uid, component);
        _audio.PlayPvs(component.FlipSound, uid);

        // This tells the server that the component's state has changed and needs to be synced to clients.
        Dirty(uid, component);
    }

    private void UpdateAppearance(EntityUid uid, CardFlipComponent component)
    {
        // This is a check to see if the entity has an AppearanceComponent. If not, we can't change its visuals.
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;
        var state = component.Flipped ? component.BackState : component.FrontState;
        _appearance.SetData(uid, CardFlipVisuals.State, state, appearance);
    }
}

public enum CardFlipVisuals : byte
{
    State
}