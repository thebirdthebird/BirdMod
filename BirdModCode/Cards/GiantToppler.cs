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

public class GiantToppler() : BirdModCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StaggeringPower>(20m),
        new IntVar("StaggeringHits", 10m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-GIANT_TOPPLER.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];
    
    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        List<StaggeringPower> list = new List<StaggeringPower>();
        var ll = play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target];
        foreach (var guy in ll)
            list = [.. guy.Powers.OfType<StaggeringPower>()];
        foreach (StaggeringPower editThis in list)
        {
            await editThis.PerformBoom(choiceContext);
        }
        
        StaggeringPower? staggering = await PowerCmd.Apply<StaggeringPower>(choiceContext, oc, this.DynamicVars["StaggeringPower"].BaseValue, oc, this, false);
        if (staggering != null)
        {
            staggering.SetHits(this.DynamicVars["StaggeringHits"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        base.DynamicVars["StaggeringPower"].UpgradeValueBy(-5m);
    }
}