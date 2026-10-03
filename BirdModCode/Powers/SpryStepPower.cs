using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Powers;

public class SpryStepPower() : BirdModPower
{
    private class Data
    {
        public int counterLeft = 30;
    }

    private const int _baseCounterLeft = 30;

    private const string _baseCounterKey = "BaseCounter";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => GetInternalData<Data>().counterLeft;

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new("BaseCounter", _baseCounterLeft)];

    protected override object InitInternalData()
    {
        return new Data();
    }
    
    public override async Task AfterPowerAmountChanged(
        PlayerChoiceContext choiceContext,
        PowerModel power,
        Decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (applier != this.Owner || base.Owner.Player == null || power is not (CounterPower or CounterAllPower))
            return;
        Data data = GetInternalData<Data>();
        for (int i = 0; i < amount; ++i)
        {
            data.counterLeft -= 1;
            if (data.counterLeft <= 0)
            {
                Flash();
                await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
                // await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), base.Owner, base.Amount, base.Owner, null);
                
                data.counterLeft = _baseCounterLeft;
                InvokeDisplayAmountChanged();
            }
        }
        InvokeDisplayAmountChanged();
    }
}