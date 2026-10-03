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
        PowerStackType.Single;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (((int)side == 2 && Owner.IsPlayer) || ((int)side == 1 && !Owner.IsPlayer))
        {
            await PowerCmd.Apply<CounterPower>(choiceContext, base.Owner, base.Owner.Block, base.Owner, null);
            await PowerCmd.Remove((PowerModel) this);
        }
    }
}