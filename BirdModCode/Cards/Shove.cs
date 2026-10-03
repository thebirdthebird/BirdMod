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
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Shove() : BirdModCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StaggeringPower>(14m),
        new IntVar("StaggeringHits", 3m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-SHOVE.flavor")),
        HoverTipFactory.FromPower<StaggeringPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        var staggering = await PowerCmd.Apply<StaggeringPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], this.DynamicVars["StaggeringPower"].BaseValue, oc, this, false);
        foreach (var pain in staggering)
        {
            pain.SetHits(this.DynamicVars["StaggeringHits"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["StaggeringPower"].UpgradeValueBy(6m);
    }
}