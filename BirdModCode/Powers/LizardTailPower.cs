using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BirdMod.BirdModCode.Powers;

public class LizardTailPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    private bool _isReviving;

    public override bool ShouldAllowHitting(Creature creature)
    {
        return (creature != Owner || !_isReviving);
    }

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return (creature != Owner || !_isReviving) && !Owner.HasPower<FairyPotionPower>();
    }

    public override bool ShouldDie(Creature creature)
    {
        return (creature != Owner && !Owner.HasPower<FairyPotionPower>());
    }
    
    public override bool ShouldStopCombatFromEnding()
    {
        return true;
    }
    
    public override bool ShouldDieLate(Creature creature)
    {
        if (creature != base.Owner && !Owner.HasPower<FairyPotionPower>())
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
        
        await CreatureCmd.Heal(Owner, Owner.MaxHp * (decimal)0.5);
        _isReviving = false;
        
        await PowerCmd.Remove(this);
    }
}