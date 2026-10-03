using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BirdMod.BirdModCode.Cards;

public class Pluck() : BirdModCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(3)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            .. HoverTipFactory.FromCardWithCardHoverTips<Feather>(IsUpgraded)
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-WARNING.text")),
            new HoverTip(new LocString("cards", "BIRDMOD-PLUCK.flavor")),
            .. HoverTipFactory.FromCardWithCardHoverTips<Feather>(IsUpgraded)
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (Owner == null) return; // if enemy, kill
        var uhmmdothething = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 0, base.DynamicVars.Cards.IntValue),
            context: choiceContext, player: base.Owner, filter: null, source: this);
        if (CombatState == null) return;
        foreach (CardModel item in uhmmdothething)
        {
            CardModel cardModel = base.CombatState.CreateCard<Feather>(base.Owner);
            if (base.IsUpgraded)
            {
                CardCmd.Upgrade(cardModel);
            }
            await CardCmd.Transform(item, cardModel);
        }
        
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(2m);
    }
}