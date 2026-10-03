using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace BirdMod.BirdModCode.Relics;

public class TalonPack() : BirdModRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Shop;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("Adroit", 2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [
            HoverTipFactory.FromCard<Talon>(),
            .. HoverTipFactory.FromEnchantment<Adroit>()
        ];

    public override async Task AfterObtained()
    {
        CardModel card = base.Owner.RunState.CreateCard<Talon>(base.Owner);
        CardCmd.Enchant<Adroit>(card, DynamicVars["Adroit"].IntValue);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);
    }
    
}