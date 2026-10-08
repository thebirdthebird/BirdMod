using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Powers;

public class AwaitPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    private int AAAA = 0;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner))
            return;
        await PowerCmd.Apply<CounterPower>(new BlockingPlayerChoiceContext(), base.Owner, AAAA, base.Owner, null);
        await PowerCmd.Decrement((PowerModel) this);
    }   
    

    public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if ((side == CombatSide.Enemy && Owner.IsPlayer) || (side == CombatSide.Player && !Owner.IsPlayer))
        {
            AAAA = Owner.Block;
        }
        return base.BeforeSideTurnEnd(choiceContext, side, participants);
    }
    
}