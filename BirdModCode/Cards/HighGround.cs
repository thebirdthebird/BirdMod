using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BirdMod.BirdModCode.Cards;

public class HighGround() : BirdModCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<HighGroundPower>(5m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-HIGH_GROUND.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await PowerCmd.Apply<HighGroundPower>(choiceContext, oc, base.DynamicVars["HighGroundPower"].BaseValue, oc, null);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HighGroundPower"].UpgradeValueBy(2m);
    }
}