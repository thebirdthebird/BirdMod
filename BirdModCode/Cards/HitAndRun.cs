using BaseLib.Cards.Variables;
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

public class HitAndRun() : BirdModCard(0,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<HitAndRunPower>(4m),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>(),
            HoverTipFactory.FromPower<VimPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-HIT_AND_RUN.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>(),
            HoverTipFactory.FromPower<VimPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (oc.IsMonster) return; // stop
        int num = ResolveEnergyXValue();
        await PowerCmd.Apply<HitAndRunPower>(choiceContext, oc,
            (base.DynamicVars["HitAndRunPower"].BaseValue * num), oc, null);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HitAndRunPower"].UpgradeValueBy(2m);
    }
}