using Robust.Shared.GameStates;

namespace Content.Shared.Cards.Components;

/// <summary>
/// This is a marker component that identifies an entity as being "flippable",
/// like a card. It relies on ItemSwitchComponent to handle the actual state logic.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class CardFlipComponent : Component
{
}