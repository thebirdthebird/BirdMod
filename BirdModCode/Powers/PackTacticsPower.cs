using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace BirdMod.BirdModCode.Powers;

public class PackTacticsPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    public override async Task AfterAutoPostPlayPhaseEntered(
        PlayerChoiceContext choiceContext,
        Player player)
    {
        CardPile hand;
        if (player != this.Owner.Player)
        {
            hand = (CardPile) null!;
        }
        else
        {
            hand = PileType.Hand.GetPile(this.Owner.Player);
            for (int i = 0; i < this.Amount; ++i)
            {
                CardModel card = this.Owner.Player.RunState.Rng.Shuffle.NextItem<CardModel>((IEnumerable<CardModel>) hand.Cards.Where<CardModel>((Func<CardModel, bool>) (c => !c.Keywords.Contains(CardKeyword.Unplayable))).ToList<CardModel>())!;
                if (card != null!)
                    await CardCmd.AutoPlay(choiceContext, card, (Creature) null!);
            }
            hand = (CardPile) null!;
        }
    }
    
}