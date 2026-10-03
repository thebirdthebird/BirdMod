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

public class Setup() : BirdModCard(1, CardType.Power,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<SetupPower>(6m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>(),
            HoverTipFactory.FromPower<CounterPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-SETUP.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>(),
            HoverTipFactory.FromPower<CounterPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await PowerCmd.Apply<SetupPower>(choiceContext, oc, base.DynamicVars["SetupPower"].BaseValue, oc, null);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["SetupPower"].UpgradeValueBy(2m);
    }
}