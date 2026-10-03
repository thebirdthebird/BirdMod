using BirdMod.BirdModCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace BirdMod.BirdModCode.Relics;

public class PetalFeather() : BirdModRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Common;
    
    public const string _hpThresholdKey = "HpThreshold";

    public bool _strengthApplied;
    

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
            new DynamicVar("HpThreshold", 50m),
            new PowerVar<StrengthPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    public bool StrengthApplied
    {
        get => _strengthApplied;
        set
        {
            AssertMutable();
            _strengthApplied = value;
        }
    }

    public override async Task AfterRoomEntered(AbstractRoom room) 
    {
        if (room is CombatRoom)
        {
            await ModifyStrengthIfNecessary();
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        StrengthApplied = false;
        base.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCurrentHpChanged(Creature creature, decimal _)
    {
        if (CombatManager.Instance.IsInProgress)
        {
            await ModifyStrengthIfNecessary();
        }
    }

    public async Task ModifyStrengthIfNecessary()
    {
        Creature creature = base.Owner.Creature;
        bool flag = (decimal)creature.CurrentHp > (decimal)creature.MaxHp * (base.DynamicVars["HpThreshold"].BaseValue / 100m);
        base.Status = ((!flag) ? RelicStatus.Active : RelicStatus.Normal);
        decimal baseValue = base.DynamicVars.Strength.BaseValue;
        if (flag && StrengthApplied)
        {
            Flash();
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), creature, -baseValue, creature, null);
            StrengthApplied = false;
        }
        else if (!flag && !StrengthApplied)
        {
            Flash();
            await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), creature, baseValue, creature, null);
            StrengthApplied = true;
        }
    }
    
}