using BaseLib.Abstracts;
using BirdMod.BirdModCode.Encounters;
using BirdMod.BirdModCode.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace BirdMod.BirdModCode.Events;

/*
public class ReflectionEvent : CustomEventModel
{
    public override EventLayoutType LayoutType => EventLayoutType.Combat;

    public override EncounterModel CanonicalEncounter => ModelDb.Encounter<ReflectionEncounter>();
    
    public override bool IsShared => true;
    
    public override bool IsAllowed(IRunState runState)
    {
        //return false;
        return (runState.CurrentActIndex > 0 && runState.Players.All((Player p) => p.Character is Character.BirdMod));
    }
    
    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(Run),
        Option(Fight)
    ];
    
    public async Task Run()
    {
        SetEventFinished(PageDescription("RUN"));
    }

    public Task Fight()
    {
        int num = 3;
        List<Reward> list = new List<Reward>(num);
        list.Add(new RelicReward(ModelDb.Relic<TribalFeather>().ToMutable(), base.Owner));
        list.Add(new RelicReward(ModelDb.Relic<PetalFeather>().ToMutable(), base.Owner));
        list.Add(new GoldReward(200, base.Owner));
        List<Reward> extraRewards = list;
        EnterCombatWithoutExitingEvent<ReflectionEncounter>(extraRewards, shouldResumeAfterCombat: false);
        return Task.CompletedTask;
    }
    
}
*/