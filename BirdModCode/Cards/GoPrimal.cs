using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace BirdMod.BirdModCode.Cards;

public class GoPrimal() : BirdModCard(1,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self), ITomeCard
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new IntVar("Swift", 2m),
        // new ExhaustiveVar(2)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromCard<TalonOld>(),
            .. HoverTipFactory.FromEnchantment<Swift>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-WARNING.text")),
            new HoverTip(new LocString("cards", "BIRDMOD-GO_PRIMAL.flavor")),
            HoverTipFactory.FromCard<TalonOld>(),
            .. HoverTipFactory.FromEnchantment<Swift>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (Owner == null) return; // just straight up give up if this is an enemy
        if (CombatState == null) return;
        List<CardModel> talons = [];
        for (int i = 0; i < DynamicVars.Cards.BaseValue; i++)
        {
            talons.Add(CombatState.CreateCard<TalonOld>(Owner));
        }
        foreach (var item in await CardPileCmd.AddGeneratedCardsToCombat(talons, PileType.Hand, Owner))
        {
            CardCmd.Enchant<Swift>(item.cardAdded, DynamicVars["Swift"].IntValue);
            CardCmd.PreviewCardPileAdd(item);
        }
        base.EnergyCost.AddThisCombat(1);
    }

    protected override void OnUpgrade()
    {
        // base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.RemoveKeyword(CardKeyword.Ethereal);
        base.DynamicVars["Swift"].UpgradeValueBy(1m);
    }
}