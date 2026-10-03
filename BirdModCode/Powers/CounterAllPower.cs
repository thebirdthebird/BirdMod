using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Powers;

public class CounterAllPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    public override async Task BeforeDamageReceived(
        PlayerChoiceContext choiceContext,
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (target != this.Owner || dealer == null || !props.IsPoweredAttack() && !(cardSource is Omnislice))
            return;
        this.Flash();
        await CreatureCmd.Damage(choiceContext, this.CombatState.GetOpponentsOf(this.Owner), (Decimal)this.Amount,
            ValueProp.Unpowered | ValueProp.SkipHurtAnim, this.Owner, (CardModel)null, (CardPlay)null);
        await PowerCmd.Remove((PowerModel)this);
    }   
    
}