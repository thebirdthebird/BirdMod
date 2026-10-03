using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BirdMod.BirdModCode.Relics;

public class RitualFeather() : BirdModRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Starter;
    
    private int _timesAncestralGuidancePlayed;
    
    [SavedProperty]
    public int TimesAncestralGuidancePlayed
    {
        get => this._timesAncestralGuidancePlayed;
        set
        {
            this.AssertMutable();
            this._timesAncestralGuidancePlayed = value;
        }
    }

    public override bool ShowCounter => false;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        ..HoverTipFactory.FromCardWithCardHoverTips<AncestralGuidance>()
    ];
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != this.Owner || !CombatManager.Instance.IsInProgress ||
            cardPlay.Card.Rarity != CardRarity.Ancient || cardPlay.Card is not AncestralGuidance)
            return;
        this.TimesAncestralGuidancePlayed++;
    }
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        ICombatState combatState)
    {
        if (player != this.Owner || this.Owner.PlayerCombatState.TurnNumber != 1)
            return;
        List<CardModel> cards = new List<CardModel>();
        for (int index = 0; index < this.DynamicVars.Cards.IntValue; ++index)
            cards.Add(this.Owner.Creature.CombatState.CreateCard<AncestralGuidance>(this.Owner));
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, this.Owner);
    }
    
    
    
}