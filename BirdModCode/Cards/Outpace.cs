using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Outpace() : BirdModCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(6m, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-OUTPACE.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        await CreatureCmd.GainBlock(oc, this.DynamicVars.Block, play);
        var dude = play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target];
        List<StaggeringPower> list = new List<StaggeringPower>();
        foreach (var guy in dude)
            list = [.. guy.Powers.OfType<StaggeringPower>()];
        foreach (StaggeringPower copyThis in list)
        {
            await CreatureCmd.GainBlock(oc, this.DynamicVars.Block, play);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(1m);
    }
}