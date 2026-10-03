using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Cards;

[Pool(typeof(StatusCardPool))]

public class Stumble() : BirdModCard(-1,
    CardType.Status, CardRarity.Status,
    TargetType.None)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<DexterityPower>(2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<DexterityPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-STAGGER.flavor")),
        HoverTipFactory.FromPower<DexterityPower>()
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Unplayable,
        CardKeyword.Ethereal
    ];

    public override async Task AfterCardDrawn(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool fromHandDraw)
    {
        if (card != this)
            return;
        await Cmd.Wait(0.25f);
        await PowerCmd.Apply<StumblePower>(choiceContext, base.Owner.Creature, base.DynamicVars["DexterityPower"].BaseValue, base.Owner.Creature, this);
    }

}