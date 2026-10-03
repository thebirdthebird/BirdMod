using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Powers;

public class VimPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override object InitInternalData() => (object) new VimPower.Data();

    public override Task BeforeBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
     
        if (creature != this.Owner || !props.IsPoweredCardOrMonsterMoveBlock())
            return Task.CompletedTask;
        VimPower.Data internalData = this.GetInternalData<VimPower.Data>();
        if (internalData.thisWillBeCardModel != null || cardSource != null && !(cardSource is CardModel) || !props.IsPoweredAttack())
            return Task.CompletedTask;
        internalData.thisWillBeCardModel = cardSource;
        internalData.amountWhenBlockStarted = this.Amount;
        internalData.theCreature = creature;
        return Task.CompletedTask;
        
    }

    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (this.Owner != target || !props.IsPoweredCardOrMonsterMoveBlock() || cardSource == null)
            return 0M;
        VimPower.Data internalData = this.GetInternalData<VimPower.Data>();
        return internalData.thisWillBeCardModel != null && cardSource != internalData.thisWillBeCardModel || internalData.thisWillBeCardModel != null && internalData.theCreature != target ? 0M : (Decimal) this.Amount;

    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        VimPower.Data internalData = this.GetInternalData<VimPower.Data>();
        if (cardPlay.Card != internalData.thisWillBeCardModel)
            return;
        internalData.thisWillBeCardModel = null;
        int num = await PowerCmd.ModifyAmount(choiceContext, (PowerModel) this, (Decimal) (-internalData.amountWhenBlockStarted), (Creature) null, (CardModel) null);
    }

    private class Data
    {
        public CardModel? thisWillBeCardModel;
        public Creature theCreature;
        public int amountWhenBlockStarted;
    }
}