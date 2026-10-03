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

public class Bolster() : BirdModCard(1,
    CardType.Skill, CardRarity.Common,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(5m, ValueProp.Move),
        new RepeatVar(2)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<CounterPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-BOLSTER.flavor")),
        HoverTipFactory.FromPower<CounterPower>()
    ];
    
    public override bool GainsBlock => true;
    
    public static Creature CalculationOwner { get; set; }

    protected override bool ShouldGlowGoldInternal => WasCounterAppliedThisTurn;

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        CalculationOwner = oc;
        int blockGains = 1;
        if (WasCounterAppliedThisTurn)
            blockGains += DynamicVars.Repeat.IntValue;
        for (int i = 0; i < blockGains; ++i)
        {
            await CreatureCmd.GainBlock(oc, DynamicVars.Block, play);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
    }
    
    private bool WasCounterAppliedThisTurn
    {
        get
        {
            return CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>().Any(e => e.HappenedThisTurn(CombatState) && e.Power is CounterPower or CounterAllPower && e.Applier == CalculationOwner);
        }
    }
}