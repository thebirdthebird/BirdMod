using BaseLib.Abstracts;
using BaseLib.Extensions;
using BirdMod.BirdModCode.Monsters;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace BirdMod.BirdModCode.Encounters;

public class ReflectionBossEncounter() : CustomEncounterModel(RoomType.Boss)
{
    private EncounterModel FakeEncounter => field ??= GetFakeEncounter();
    private EncounterModel GetFakeEncounter() => RunManager.Instance.DebugOnlyGetState()?.Rng.Niche.NextInt(3) switch
    {
        0 => ModelDb.Encounter<QueenBoss>(),
        1 => ModelDb.Encounter<TestSubjectBoss>(),
        2 => ModelDb.Encounter<AeonglassBoss>(),
        _ => ModelDb.Encounter<QueenBoss>()
    };
    
    public override string BossNodePath => FakeEncounter.BossNodePath;
    public override MegaSkeletonDataResource? BossNodeSpineResource => FakeEncounter.BossNodeSpineResource;
    
    public override IReadOnlyList<string> Slots =>
    [
        "reflection"
    ];
    public override float GetCameraScaling()
    {
        return 1.35f;
    }
    public override Vector2 GetCameraOffset()
    {
        return (Vector2.Left * 90f + Vector2.Up * 120f);
    }
    public override bool IsValidForAct(ActModel act)
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once IdentifierTypo
        var OHMYGODFUCKYOUSTUPIDCODE = RunManager.Instance.DebugOnlyGetState();
        if (OHMYGODFUCKYOUSTUPIDCODE == null)
            return act.ActNumber() == 3;
        return act.ActNumber() == 3 &&
               OHMYGODFUCKYOUSTUPIDCODE.Players.Any((Player p) => p.Character is Character.BirdMod);
    }
    
    //public override string CustomBgm => "res://BirdMod/music/CrimsonTalons.ogg"; //doesnt work
    public override string? CustomScenePath => "res://BirdMod/images/encounters/test_encounter.tscn";

    public override IEnumerable<MonsterModel> AllPossibleMonsters => 
    [
        ModelDb.Monster<ReflectionEnemy>()
    ];
    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
        (ModelDb.Monster<ReflectionEnemy>().ToMutable(), "reflection"),
    ];
    
    public override string? CustomRunHistoryIconOutlinePath => ImageHelper.GetImagePath($"ui/run_history/{FakeEncounter.Id.Entry.ToLowerInvariant()}_outline.png");
    public override string? CustomRunHistoryIconPath => ImageHelper.GetImagePath($"ui/run_history/{FakeEncounter.Id.Entry.ToLowerInvariant()}.png");
}
