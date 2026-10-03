using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BirdMod.BirdModCode.Powers;

public class MirrorMovePower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Player == null || cardPlay.Player == Owner.Player || cardPlay.Card is MirrorMove) return;
        var target = cardPlay.Target is { IsDead: false } t ? t : null;
        if (cardPlay.Card.TargetType == TargetType.Self)
        {
            target = base.Owner;
        }
        await CardCmd.AutoPlay(choiceContext, cardPlay.Card.CreateCloneForPlayer(base.Owner.Player), target);
        await PowerCmd.Decrement(this);
    }
}