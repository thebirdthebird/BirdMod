using BaseLib.Utils;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class OblivionWing() : BirdModCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(7, ValueProp.Move),
        new RepeatVar(2),
        new PowerVar<VimPower>(7m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<VimPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-OBLIVION_WING.flavor")),
            HoverTipFactory.FromPower<VimPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (CombatState == null) return;
        Decimal damageNum = (await DamageCmd.
                Attack(this.DynamicVars.Damage.BaseValue)
                .FromCard(this, play)
                .TargetingAllOpponents(CombatState)
                .WithHitFx("vfx/vfx_shiv_throw", "res://BirdMod/sounds/snd_knight_cut.wav")
                .WithHitCount(DynamicVars.Repeat.IntValue)
                .Execute(choiceContext))
            .Results
            .SelectMany(r => r)
            .Sum((Func<DamageResult, int>)(r => r.TotalDamage));
        // sorry damage num you are now UNUSED!!!!
        await PowerCmd.Apply<VimPower>(choiceContext, oc, base.DynamicVars["VimPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars["VimPower"].UpgradeValueBy(3m);
    }
}