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

public class IronWing() : BirdModCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4, ValueProp.Move),
        new DamageVar(4, ValueProp.Move),
        new PowerVar<CounterAllPower>(2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<CounterAllPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-IRON_WING.flavor")),
            HoverTipFactory.FromPower<CounterAllPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        await CreatureCmd.GainBlock(oc, base.DynamicVars.Block, play);
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .HelpTheCmd(oc, play, this, Owner)
            .Execute(choiceContext);
        await PowerCmd.Apply<CounterAllPower>(choiceContext, oc, base.DynamicVars["CounterAllPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(1m);
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["CounterAllPower"].UpgradeValueBy(1m);
    }
}