using Content.Shared.Cards.Components;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Starlight.ItemSwitch;
using Robust.Shared.Audio.Systems;

namespace Content.Shared.Cards.Systems;

public sealed partial class SharedCardFlipSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedItemSwitchSystem _itemSwitch = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CardFlipComponent, ItemSwitchedEvent>(OnItemSwitched);
    }

    private void OnItemSwitched(Entity<CardFlipComponent> ent, ref ItemSwitchedEvent args)
    {
        UpdateAppearance(ent, args.State);
    }

    private void UpdateAppearance(EntityUid uid, string state)
    {
        // This is a check to see if the entity has an AppearanceComponent. If not, we can't change its visuals.
        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;
        _appearance.SetData(uid, CardFlipVisuals.State, state);
        
    }
}