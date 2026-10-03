using BirdMod.BirdModCode.Cards;
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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;


public class Twirl() : BirdModCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.Self)
{
    
    private const string _replayKey = "Replay";
    
    protected override bool ShouldGlowGoldInternal => this.WasCounterAppliedThisTurn;
    
    private bool WasCounterAppliedThisTurn
    {
        get
        {
            return CombatManager.Instance.History.Entries.OfType<PowerReceivedEntry>().Any<PowerReceivedEntry>((Func<PowerReceivedEntry, bool>) (e => e.HappenedThisTurn(this.CombatState) && e.Power is CounterPower or CounterAllPower && e.Applier == this.Owner.Creature));
        }
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<CounterPower>(),
            HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-TWIRL.flavor")),
            HoverTipFactory.FromPower<CounterPower>(),
            HoverTipFactory.Static(StaticHoverTip.ReplayStatic)
        ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(7, ValueProp.Move),
        new IntVar("Replay", 1m),
        new IntVar("HowManyTimes", 1m)
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await CreatureCmd.GainBlock(oc, this.DynamicVars.Block, play);
        if (Owner == null) return;
        for (int i = 0; i < DynamicVars["HowManyTimes"].IntValue; ++i)
        {
            List<CardModel> list = PileType.Draw.GetPile(base.Owner).Cards.ToList();
            if (list.Count == 0)
            {
                return;
            }
            List<CardModel> list2 = list.Where(delegate(CardModel c)
            {
                bool flag = !c.Keywords.Contains(CardKeyword.Unplayable);
                bool flag2 = flag;
                if (flag2)
                {
                    CardType type = c.Type;
                    bool flag3 = (uint)(type - 5) <= 1u;
                    flag2 = !flag3;
                }
                return flag2 && c.GetEnchantedReplayCount() < 1;
            }).ToList();
            List<CardModel> list3 = list2.Where(delegate(CardModel c)
            {
                CardType type = c.Type;
                return (uint)(type - 1) <= 2u;
            }).ToList();
            IEnumerable<CardModel> items = ((list3.Count == 0) ? list2 : list3);
            CardModel? cardModel = base.Owner.RunState.Rng.CombatCardSelection.NextItem(items);
            if (cardModel != null  && this.WasCounterAppliedThisTurn )
            {
                cardModel.BaseReplayCount += base.DynamicVars["Replay"].IntValue;
                CardCmd.Preview(cardModel);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
        base.DynamicVars["HowManyTimes"].UpgradeValueBy(1m);
    }
}