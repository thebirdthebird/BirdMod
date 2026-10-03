using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BirdMod.BirdModCode.Cards;

public class Yank() : BirdModCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new IntVar("IncHits", 1m),
        new IntVar("IncDmg", 12m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-YANK.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        var dude = play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target];
        List<StaggeringPower> list = new List<StaggeringPower>();
        foreach (var guy in dude)
            list = [.. guy.Powers.OfType<StaggeringPower>()];
        
        foreach (StaggeringPower editThis in list)
        {
            editThis.SetAmount(editThis.Amount + this.DynamicVars["IncDmg"].IntValue);
            editThis.SetHits(editThis.GetHits() + this.DynamicVars["IncHits"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["IncDmg"].UpgradeValueBy(6m);
    }
}