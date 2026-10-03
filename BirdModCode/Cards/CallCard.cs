using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BirdMod.BirdModCode.Cards;

public abstract class CallCard(int cost, CardType type, CardRarity rarity, TargetType target)
    : BirdModCard(cost, type, rarity, target)
{
    public abstract Task OnAncestralCall(
        PlayerChoiceContext choiceContext, 
        CardPlay play, Creature ownerCreature);

    public sealed override Task SuperPlay(
        PlayerChoiceContext choiceContext, 
        CardPlay play, Creature ownerCreature)
    {
        // dude? how?
        return OnAncestralCall(choiceContext, play, ownerCreature);
    }
}