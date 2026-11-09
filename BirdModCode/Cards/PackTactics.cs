using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BirdMod.BirdModCode.Cards;

public class PackTactics() : BirdModCard(3,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        var selection = (await CardSelectCmd.FromCombatPile(choiceContext, PileType.Draw.GetPile(base.Owner), base.Owner, new CardSelectorPrefs(SelectionScreenPrompt, DynamicVars.Cards.IntValue, DynamicVars.Cards.IntValue))).ToList();
        foreach (var card in selection)
        {
            await CardCmd.AutoPlay(choiceContext, card, null!);
            if (card.Keywords.Contains(CardKeyword.Exhaust) || card.Type == CardType.Power) continue;
            await CardCmd.Exhaust(choiceContext, card);
            var power = await PowerCmd.Apply<ChosenPackTacticPower>(choiceContext, base.Owner.Creature, 1m,
                base.Owner.Creature, this);
            if (power != null && card != null!)
                power.SetSelectedCard(card);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}