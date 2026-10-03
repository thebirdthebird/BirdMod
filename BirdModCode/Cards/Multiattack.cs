using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Multiattack() : BirdModCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new RepeatVar(2),
        new IntVar("HitsIfCounter", 1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<CounterPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-MULTIATTACK.flavor")),
            HoverTipFactory.FromPower<CounterPower>()
        ];
    
    public static Creature CalculationOwner { get; set; }
    
    protected override bool ShouldGlowGoldInternal => this.WasCounterAppliedThisTurn;

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        CalculationOwner = oc;
        int num = DynamicVars.Repeat.IntValue;
        if (play != null)
            if (play.Target == null)
                return;
        if (this.WasCounterAppliedThisTurn) num += DynamicVars["HitsIfCounter"].IntValue;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .HelpTheCmd(oc, play, this, Owner)
            .WithHitCount(num)
            .Execute(choiceContext);
    }
    
    private bool WasCounterAppliedThisTurn
    {
        get
        {
            return CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>().Any<PowerReceivedEntry>((Func<PowerReceivedEntry, bool>) (e => e.HappenedThisTurn(this.CombatState) && e.Power is CounterPower or CounterAllPower && e.Applier == CalculationOwner));
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["HitsIfCounter"].UpgradeValueBy(1m);
    }
}