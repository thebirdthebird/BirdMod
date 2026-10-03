using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Powers;

public class TailwindPower() : CallablePower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task YouGotCalled(PlayerChoiceContext choiceContext, int hitsThatItLasted, Creature? owner)
    {
        if (base.Owner.Player == null) return;
        this.Flash();
        await CardPileCmd.Draw(choiceContext, base.Amount, base.Owner.Player);
    }
    
}