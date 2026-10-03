using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BirdMod.BirdModCode.Powers;

public class CurbstompPower() : CallablePower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task YouGotCalled(PlayerChoiceContext choiceContext, int hitsThatItLasted, Creature? agghhh)
    {
        if (base.Owner.Player == null) return;
        if (agghhh == null) return;
        this.Flash();
        await PowerCmd.Apply<CurbstompStrPower>(choiceContext, agghhh, base.Amount * hitsThatItLasted, base.Owner, null);
    }
}