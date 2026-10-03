using BirdMod.BirdModCode.Extensions;
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

public class WingStrike() : BirdModCard(2,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move),
        new RepeatVar(2),
        new IntVar("StaggeringHits", 5m)
    ];
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-WING_STRIKE.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];


    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        var dmg = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .HelpTheCmd(oc, play, this, Owner)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .Execute(choiceContext);
        var i = dmg.Results
            .SelectMany(
                r => r)
            .Sum(r => r.TotalDamage);
        var staggering = await PowerCmd.Apply<StaggeringPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], i, oc, this);
        foreach (var pain in staggering)
        {
            pain.SetHits(DynamicVars["StaggeringHits"].IntValue);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}