using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Powers;

public class AncestralProtectionPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (applier != this.Owner || power is not (CounterPower or CounterAllPower))
            return;
        
        foreach (Creature hittableEnemy in this.CombatState.GetOpponentsOf(this.Owner))
        {
            await PowerCmd.Apply<AncestralProtectionStrPower>(choiceContext, hittableEnemy, this.Amount, this.Owner, null);
        }
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner) || this.AmountOnTurnStart == 0)
            return;
        await PowerCmd.Remove((PowerModel) this);
    }
}