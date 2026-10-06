using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace BirdMod.BirdModCode.Cards;

public class GoFeral() : BirdModCard(3,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<GoFeralPower>(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.Static(StaticHoverTip.Transform),
            // HoverTipFactory.FromCard<Talon>(),
            HoverTipFactory.FromCard<Feather>(),
            .. HoverTipFactory.FromEnchantment<Swift>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-GO_FERAL.flavor")),
            HoverTipFactory.Static(StaticHoverTip.Transform),
            // HoverTipFactory.FromCard<Talon>(),
            HoverTipFactory.FromCard<Feather>(),
            .. HoverTipFactory.FromEnchantment<Swift>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await PowerCmd.Apply<GoFeralPower>(choiceContext, oc, base.DynamicVars["GoFeralPower"].BaseValue, oc, null);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}