using System.Numerics;
using Content.Shared.Projectiles;
using Content.Shared.Weapons.Misc;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Starlight.Aquean;

/// <summary>
/// This handles the logic for the Aquean's sticky tongue ability, which functions as a short-range grapple.
/// </summary>
public sealed class AqueanStickyTongueSystem : EntitySystem
{
    [Dependency] private readonly SharedJointSystem _joints = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AqueanStickyTongueComponent, ProjectileEmbedEvent>(OnTongueHit);
    }

    private void OnTongueHit(Entity<AqueanStickyTongueComponent> ent, ref ProjectileEmbedEvent args)
    {
        if (!TryComp<PhysicsComponent>(ent, out var physics))
            return;

        var jointComp = EnsureComp<JointComponent>(ent);
        var joint = _joints.CreateDistanceJoint(ent, args.Weapon, anchorA: Vector2.Zero, id: SharedGrapplingGunSystem.GrapplingJoint);
        joint.MaxLength = joint.Length;
        joint.Stiffness = 1f;
        joint.MinLength = 0.35f;
        Dirty(ent, jointComp);
    }
}

[RegisterComponent, Serializable, Access(typeof(AqueanStickyTongueSystem))]
public sealed partial class AqueanStickyTongueComponent : Component
{
}
