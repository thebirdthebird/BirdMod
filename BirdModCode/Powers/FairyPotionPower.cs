using BirdMod.BirdModCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace BirdMod.BirdModCode.Powers;

public class FairyPotionPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    private bool _isReviving;

    public override bool ShouldAllowHitting(Creature creature)
    {
        return creature != Owner || !_isReviving;
    }

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return creature != Owner || !_isReviving;
    }

    public override bool ShouldDie(Creature creature)
    {
        return creature != Owner;
    }
    
    public override bool ShouldStopCombatFromEnding()
    {
        return true;
    }
    
    public override bool ShouldDieLate(Creature creature)
    {
        if (creature != base.Owner)
        {
            return true;
        }
        return false;
    }
    
    public override bool ShouldPowerBeRemovedAfterOwnerDeath()
    {
        return false;
    }
    
    public override async Task AfterPreventingDeath(Creature creature)
    {
        Flash();
        _isReviving = true;

        var packedImagePath =
            ImageHelper.GetImagePath("atlases/potion_atlas.sprites/fairy_in_a_bottle.tres");
        var image = ResourceLoader.Load<Texture2D>(packedImagePath);
        
        NCreature? nCreature = NCombatRoom.Instance?.GetCreatureNode(Owner);
        Vector2 val = (NCombatRoom.Instance?.GetCreatureNode(Owner))?.GetBottomOfHitbox() ?? Vector2.Zero;
        
        Vector2 sourcePosition = nCreature?.VfxSpawnPosition ?? Vector2.Zero;
        NItemThrowVfx? child = NItemThrowVfx.Create(sourcePosition, val, image);
        ((Node)(object)NCombatRoom.Instance?.CombatVfxContainer!).AddChildSafely((Node?)(object)child!);
        await Cmd.Wait(0.5f);
        
        await CreatureCmd.Heal(Owner, Owner.MaxHp * (decimal)0.3);
        _isReviving = false;


        await PowerCmd.Decrement(this);
    }
    
}