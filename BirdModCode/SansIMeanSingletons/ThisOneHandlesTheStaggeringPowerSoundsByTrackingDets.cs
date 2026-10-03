using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace BirdMod.BirdModCode.SansIMeanSingletons;

public class ThisOneHandlesTheStaggeringPowerSoundsByTrackingDets() : CustomSingletonModel(HookType.Combat)
{
    private static int _dets = 0;
    public static int GetDets() => _dets;
    public static void SetDets(int detsSet) => _dets = detsSet;
    public static void IncDets() => _dets++;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        _dets = 0;
    }
}