using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Powers;

public class DodgePower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;
    
    public override Decimal ModifyHpLostAfterOsty(
        Creature target,
        Decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        return !CombatManager.Instance.IsInProgress || target != this.Owner || amount < 1M || dealer == null || dealer == this.Owner || !props.IsPoweredAttack() ? amount : 0M;
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        this.Flash();
        await PowerCmd.Decrement((PowerModel) this);
    }

    /// <summary>
    /// Caps damage received at 0.
    /// Note: the HP loss logic is already handled by modifyhplostafterosty, the duplicated logic here is
    /// for block loss and preview logic on targeted attacks.
    /// </summary>
    public override Decimal ModifyDamageCap(
        Creature? target,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        CardPlay? cardPlay)
    {
        return target != this.Owner || dealer == null || dealer == this.Owner || !props.IsPoweredAttack() ? Decimal.MaxValue : 0M;
    }

    /// <summary>
    /// Note: the HP loss logic is already handled by modifydamagecap, the duplicated logic
    /// here is for block loss and preview logic on targeted attacks.
    /// </summary>
    public override async Task AfterModifyingDamageAmount(CardModel? cardSource)
    {
        this.Flash();
        await PowerCmd.Decrement((PowerModel) this);
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner) || this.AmountOnTurnStart == 0)
            return;
        await PowerCmd.Remove((PowerModel) this);
    }
    
}