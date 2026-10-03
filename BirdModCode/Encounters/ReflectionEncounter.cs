using BaseLib.Abstracts;
using BirdMod.BirdModCode.Monsters;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BirdMod.BirdModCode.Encounters;

public class ReflectionEncounter : CustomEncounterModel
{
    public ReflectionEncounter() : base(RoomType.Monster)
    {
    }
    public override IReadOnlyList<string> Slots =>
    [
        "reflection"
    ];
    public override bool IsValidForAct(ActModel act)
    {
        return false;
    }
    public override float GetCameraScaling()
    {
        return 1.35f;
    }
    public override Vector2 GetCameraOffset()
    {
       return (Vector2.Left * 90f + Vector2.Up * 120f);
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
}