using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.SansIMeanSingletons;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Talon() : BirdModCard(1, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    private decimal ExtraDamageFromTalon
    {
        get;
        set
        {
            this.AssertMutable();
            field = value;
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move),
        new RepeatVar(2),
        new DynamicVar("Increase", 1m),
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_scratch", "res://BirdMod/sounds/snd_knight_cut2.wav")
            .HelpTheCmd(oc, play, this, Owner)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .Execute(choiceContext);
        if (Owner != null) TalonScaling.ScaleDamage((int)DynamicVars["Increase"].BaseValue, this.Owner.PlayerCombatState);
        
        // ORIGINAL CODE THAT SCALES LIKE CLAW:
        /*
        if (this.Owner.PlayerCombatState == null) return;
        IEnumerable<Talon> enumerable = this.Owner.PlayerCombatState.AllCards.OfType<Talon>();
        decimal baseValue = this.DynamicVars["Increase"].BaseValue;
        foreach (Talon item in enumerable)
        {
            item.BuffFromTalonPlay(baseValue);
        }
        */
    }
    
    public void BuffFromTalonPlay(decimal extraHits)
    {
        RepeatVar repeat = this.DynamicVars.Repeat;
        repeat.BaseValue += extraHits;
        ExtraDamageFromTalon += extraHits;
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
    }
}