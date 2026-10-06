using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Cards;

public class Focus() : BirdModCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VigorPower>(3m),
        new PowerVar<VimPower>(5m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<VimPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-FOCUS.flavor")),
        HoverTipFactory.FromPower<VigorPower>(),
        HoverTipFactory.FromPower<VimPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await PowerCmd.Apply<VigorPower>(choiceContext, oc, base.DynamicVars["VigorPower"].BaseValue, oc, this);
        await PowerCmd.Apply<VimPower>(choiceContext, oc, base.DynamicVars["VimPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["VigorPower"].UpgradeValueBy(2m);
        base.DynamicVars["VimPower"].UpgradeValueBy(3m);
    }
}