using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Powers;

public class SpiritSummonPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;

    public override async Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel? cardSource)
    {
        if (!(amount <= 0m) && creature == base.Owner)
        {
            Flash();
            await CreatureCmd.GainBlock(this.Owner.PetOwner?.Creature ?? throw new InvalidOperationException("THIS IS ONLY MEANT FOR PETS!!! We tried to get the Pet Owner of this power and EXPLODED!!!!"), amount, ValueProp.Unpowered, null);
        }
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (!(amount <= 0m) && power.Owner == base.Owner && power.Type == PowerType.Buff && power is not SpiritSummonPower)
        {
            // (PlayerChoiceContext choiceContext, PowerModel power, Creature target, decimal amount, Creature? applier, CardModel? cardSource, bool silent = false)
            Flash();
            var p = (PowerModel)power.MutableClone();
            p.Owner = this.Owner.PetOwner?.Creature ?? throw new InvalidOperationException("THIS IS ONLY MEANT FOR PETS!!! STOPPPP");
            await PowerCmd.Apply(new BlockingPlayerChoiceContext(), p, this.Owner.PetOwner?.Creature ?? throw new InvalidOperationException("THIS IS ONLY MEANT FOR PETS!!! STOPPPP"), amount, base.Owner, null);
        }
    }
}