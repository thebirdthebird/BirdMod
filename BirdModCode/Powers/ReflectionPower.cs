using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BirdMod.BirdModCode.Powers;

public class ReflectionPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.None;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
}