using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BirdMod.BirdModCode.Powers;

public class HarnessingEnergyPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar(1)
    ];

    public override async Task AfterEnergyReset(Player player)
    {
        if (player.Creature != Owner)
            return;
        
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, player);
        DynamicVars.Energy.UpgradeValueBy(Amount); //YOU CAN JUST DO THIS? THANK YOU LYALLI?
        this.Flash();
    }
    
}