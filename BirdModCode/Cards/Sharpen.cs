using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Cards;

public class Sharpen() : BirdModCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(1m),
        new PowerVar<VigorPower>(4m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<VigorPower>(),
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-SHARPEN.flavor")),
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<VigorPower>(),
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, oc, base.DynamicVars["StrengthPower"].BaseValue, oc, this);
        await PowerCmd.Apply<VigorPower>(choiceContext, oc, base.DynamicVars["VigorPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["StrengthPower"].UpgradeValueBy(1m);
        base.DynamicVars["VigorPower"].UpgradeValueBy(2m);
    }
}