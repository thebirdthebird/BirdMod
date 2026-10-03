using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace BirdMod.BirdModCode.Powers;

public class DrawLessNextTurnPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Debuff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override Decimal ModifyHandDraw(Player player, Decimal count)
    {
        return player != this.Owner.Player || this.AmountOnTurnStart == 0 ? count : count - (Decimal) this.Amount;
    }

    public override async Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner) || this.AmountOnTurnStart == 0)
            return;
        await PowerCmd.Remove((PowerModel) this);
    }
    
}