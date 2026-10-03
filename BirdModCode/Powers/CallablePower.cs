using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BirdMod.BirdModCode.Powers;

public abstract class CallablePower() : BirdModPower
{
    public abstract Task YouGotCalled(PlayerChoiceContext choiceContext, int hitsThatItLasted, Creature? owner);
    
}