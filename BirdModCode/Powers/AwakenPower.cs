using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Powers;

public class AwakenPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    private bool _isReviving;

    private decimal _mostRecentDamage = -1;
    
    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target == this.Owner) _mostRecentDamage = amount;
    }

    public override bool ShouldAllowHitting(Creature creature)
    {
        return (creature != Owner || !_isReviving);
    }

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return (creature != Owner || !_isReviving);
    }

    public override bool ShouldDie(Creature creature)
    {
        return (creature != Owner);
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
        if (_mostRecentDamage < 0) _mostRecentDamage = 1;
        await CreatureCmd.Heal(Owner, _mostRecentDamage);
        _isReviving = false;
        
        await PowerCmd.Decrement(this);
    }
}