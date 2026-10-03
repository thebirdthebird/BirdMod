using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Powers;

public class StormfrontPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => 
        [
            HoverTipFactory.FromPower<DexterityPower>()
        ];
    
    public override async Task AfterPowerAmountChanged
    (PlayerChoiceContext choiceContext, 
        PowerModel power, 
        decimal amount, 
        Creature? applier,
        CardModel? cardSource)
    {
        if (applier != this.Owner || power is not DexterityPower || amount < 1)
            return;
        if (cardSource == null) return;
        if (Owner.CombatState == null) return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(Owner)
            where c != null && c.IsAlive && c.IsPlayer
            select c;
        foreach (Creature creature in enumerable)
        {
            await PowerCmd.Apply<StormfrontDexPower>(choiceContext, creature, amount * this.Amount, Owner, null);
        }
        Flash();
    }
    
}