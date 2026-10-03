using BaseLib.Utils;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Slide() : BirdModCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(6, ValueProp.Move),
        // new PowerVar<CounterPower>(6M),
        // new CardsVar(1),
        new PowerVar<StaggeringPower>(6m),
        new IntVar("StaggeringHits", 2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-SLIDE.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (CombatState == null) return;
        var cmd = DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .HelpTheCmd(oc, play, this, Owner);
            //.WithHitCount(DynamicVars.Repeat.IntValue);
        if (!oc.IsMonster) cmd.TargetingAllOpponents(CombatState);
        await cmd.Execute(choiceContext);
        foreach (Creature hittableEnemy in this.CombatState.GetOpponentsOf(oc))
        {
            StaggeringPower? staggering = await PowerCmd.Apply<StaggeringPower>(choiceContext, hittableEnemy, this.DynamicVars["StaggeringPower"].BaseValue, oc, this, false);
            if (staggering != null)
            {
                staggering.SetHits(this.DynamicVars["StaggeringHits"].IntValue);
            }
        }
        // await PowerCmd.Apply<CounterPower>(choiceContext, base.Owner.Creature, base.DynamicVars["CounterPower"].BaseValue, base.Owner.Creature, this);
        // await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        // base.DynamicVars["CounterPower"].UpgradeValueBy(3m);
        base.DynamicVars["StaggeringPower"].UpgradeValueBy(2m);
    }
}