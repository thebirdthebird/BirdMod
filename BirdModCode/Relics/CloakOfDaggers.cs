using BirdMod.BirdModCode.Powers;
using BirdMod.BirdModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Relics;

public class CloakOfDaggers() : BirdModRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromCard<Shiv>()
    ];

    public override async Task AfterPowerAmountChanged
    (PlayerChoiceContext choiceContext, 
        PowerModel power, 
        decimal amount, 
        Creature? applier,
        CardModel? cardSource)
    {
        if (applier != this.Owner.Creature || power is not DexterityPower || amount < 1)
            return;
        if (cardSource == null) return;
        if (this.Owner.Creature.CombatState == null) return;
        List<CardModel> cards = new List<CardModel>();
        for (int index = 0; index < this.DynamicVars.Cards.IntValue; ++index)
            cards.Add(this.Owner.Creature.CombatState.CreateCard<Shiv>(this.Owner));
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, this.Owner);
        Flash();
    }
}