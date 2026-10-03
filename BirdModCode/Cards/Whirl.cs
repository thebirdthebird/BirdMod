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

public class Whirl() : BirdModCard(0, CardType.Skill,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override bool HasEnergyCostX => true;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StaggeringPower>(7m),
        new IntVar("StaggeringHits", 3m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-WHIRL.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (Owner == null) return; // give up
        if (play.Target == null) return;
        int num = ResolveEnergyXValue();
        for (int index = 0; index < num; ++index)
        {
            StaggeringPower? staggering = await PowerCmd.Apply<StaggeringPower>(choiceContext, play.Target, (this.DynamicVars["StaggeringPower"].BaseValue * num), oc, this, false);
            if (staggering != null)
            {
                staggering.SetHits(this.DynamicVars["StaggeringHits"].IntValue * num);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["StaggeringPower"].UpgradeValueBy(2m);
        base.DynamicVars["StaggeringHits"].UpgradeValueBy(-1m);
    }
}