using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Warn() : CallCard(-1, CardType.Skill,
    CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<DexterityPower>(6m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<DexterityPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-WARN.flavor")),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    public override async Task OnAncestralCall(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature ownerCreature)
    {
        await PowerCmd.Apply<WarnPower>(choiceContext, ownerCreature, base.DynamicVars["DexterityPower"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DexterityPower"].UpgradeValueBy(3m);
    }
}