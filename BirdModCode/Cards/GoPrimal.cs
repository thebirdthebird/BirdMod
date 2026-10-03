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

public class GoPrimal() : BirdModCard(0,
    CardType.Skill, CardRarity.Event,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(2),
        new IntVar("Swift", 2m),
        new ExhaustiveVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromCard<Talon>(),
            .. HoverTipFactory.FromEnchantment<Swift>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-WARNING.text")),
            new HoverTip(new LocString("cards", "BIRDMOD-GO_PRIMAL.flavor")),
            HoverTipFactory.FromCard<Talon>(),
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
            talons.Add(CombatState.CreateCard<Talon>(Owner));
        }
        foreach (var item in await CardPileCmd.AddGeneratedCardsToCombat(talons, PileType.Draw, Owner))
        {
            CardCmd.Enchant<Swift>(item.cardAdded, DynamicVars["Swift"].IntValue);
            CardCmd.PreviewCardPileAdd(item);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars["Swift"].UpgradeValueBy(1m);
    }
}