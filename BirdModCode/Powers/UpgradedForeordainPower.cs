using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Powers;

public class UpgradedForeordainPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner) || this.AmountOnTurnStart == 0)
            return;
        if (this.Amount - 1 == 0)
        {
            await PowerCmd.Apply<IntangiblePower>(new ThrowingPlayerChoiceContext(), base.Owner, 1, base.Owner, null);
        }
        await PowerCmd.Decrement(this);
    }
    
}