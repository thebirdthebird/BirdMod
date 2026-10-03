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

public class QuickStrike() : BirdModCard(1,
    CardType.Attack, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move),
        new EnergyVar(1),
    ];
    
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };

    protected override bool ShouldGlowGoldInternal => WasCounterAppliedThisTurn;
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<CounterPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-QUICK_STRIKE.flavor")),
            HoverTipFactory.FromPower<CounterPower>()
        ];
    
    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                .WithHitFx("vfx/vfx_attack_slash")
                .HelpTheCmd(oc, play, this, Owner)
                .Execute(choiceContext);
        if (Owner == null) return; //k ill
        if (WasCounterAppliedThisTurn)
            await PlayerCmd.GainEnergy((Decimal) this.DynamicVars.Energy.IntValue, this.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
    
    private bool WasCounterAppliedThisTurn
    {
        get
        {
            return CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>().Any(e => e.HappenedThisTurn(CombatState) && e.Power is CounterPower or CounterAllPower && e.Applier == Owner.Creature);
        }
    }
}