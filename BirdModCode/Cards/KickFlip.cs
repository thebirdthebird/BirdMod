using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class KickFlip() : BirdModCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3, ValueProp.Move),
        new RepeatVar(2),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<VimPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-KICK_FLIP.flavor")),
        new HoverTip(new LocString("cards", "BIRDMOD-KICK_FLIP.credits")),
        HoverTipFactory.FromPower<VimPower>()
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
        await PowerCmd.Apply<VimPower>(choiceContext, oc, i, oc, (CardModel) this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}