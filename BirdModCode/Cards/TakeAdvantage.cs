using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BirdMod.BirdModCode.Cards;

public class TakeAdvantage() : BirdModCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StaggeringPower>(15m),
        new IntVar("StaggeringHits", 5m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-TAKE_ADVANTAGE.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
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
        var dude = play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target];
        List<StaggeringPower> list = new List<StaggeringPower>();
        foreach (var guy in dude)
            list = [.. guy.Powers.OfType<StaggeringPower>()];
        // List<StaggeringPower> list = [.. play.Target.Powers.OfType<StaggeringPower>()];
        foreach (StaggeringPower copyThis in list)
        {
            StaggeringPower? stagger = await PowerCmd.Apply<StaggeringPower>(choiceContext, copyThis.Owner, copyThis.Amount, oc, this, false);
            if (stagger != null)
            {
                stagger.SetHits(copyThis.GetHits());
            }
        }
        
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["StaggeringPower"].UpgradeValueBy(5m);
    }
}