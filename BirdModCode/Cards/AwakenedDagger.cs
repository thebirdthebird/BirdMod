using BaseLib.Abstracts;
using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class AwakenedDagger() : BirdModCard(1,
    CardType.Attack, CardRarity.Ancient,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(15, ValueProp.Move),
        new PowerVar<StrengthPower>(1m)
    ];
    public override bool CanBeGeneratedInCombat => false;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StrengthPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-AWAKENED_DAGGER.flavor")),
            new HoverTip(new LocString("cards", "BIRDMOD-AWAKENED_DAGGER.credits")),
            HoverTipFactory.FromPower<StrengthPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature ownerCreature)
    {
        if (play != null)
            if (play.Target == null)
                return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                .WithHitFx("vfx/vfx_attack_slash")
                .HelpTheCmd(ownerCreature, play, this, Owner)
                .Execute(choiceContext);
        // cmd.Attacker = ownerCreature;
        // cmd._attackerAnimName = "Attack";
        // cmd.ModelSource = this;
        // if (play != null) 
            // cmd.CardPlay = play;
        // cmd._sourceType = AttackCommand.SourceType.Card;
        // if (Owner != null) cmd._attackerAnimDelay = Owner.Character.AttackAnimDelay;
        // if (ownerCreature.CombatState == null) return; //die?
        // if (ownerCreature.Monster != null) cmd.TargetingAllOpponents(ownerCreature.CombatState);
        // else
        // {
        //   if (play != null)
        //        if (play.Target != null)
        //            cmd.Targeting(play.Target);
        // }
        await PowerCmd.Apply<StrengthPower>(choiceContext, ownerCreature, base.DynamicVars["StrengthPower"].BaseValue, ownerCreature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(5m);
        base.DynamicVars["StrengthPower"].UpgradeValueBy(1m);
    }
}