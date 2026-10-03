using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Flurry() : BirdModCard(1, CardType.Attack,
    CardRarity.Uncommon, TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move),
        new RepeatVar(2),
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (CombatState == null) return;
        var cmd = DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .HelpTheCmd(oc, play, this, Owner)
            .WithHitCount(DynamicVars.Repeat.IntValue);
        if (!oc.IsMonster) cmd.TargetingAllOpponents(CombatState);
        await cmd.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Repeat.UpgradeValueBy(1m);
    }
}