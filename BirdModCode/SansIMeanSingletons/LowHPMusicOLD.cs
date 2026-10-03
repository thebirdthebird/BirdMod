using BaseLib.Abstracts;
using BirdMod.BirdModCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves;

namespace BirdMod.BirdModCode.SansIMeanSingletons;

/*
[HarmonyPatch]
public class LowHPMusicOLD() : CustomSingletonModel(HookType.Combat)
{
    private static readonly Dictionary<string, AudioStreamOggVorbis> CachedStreams = new Dictionary<string, AudioStreamOggVorbis>();
    
    const float SilenceDb = -40f;
    const float FadeSeconds = 1.5f;
    
    static AudioStreamPlayer? _player;
    static bool _low;
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.UpdateMusic))]
    static bool BlockUpdateMusic() => !_low;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.PlayCustomMusic))]
    static bool BlockCustomMusic() => !_low;

    public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!LocalContext.IsMe(creature) || NRun.Instance == null || !BirdModConfig.CloseToDeathMusic)
            return;
        var low = creature.CurrentHp > 0 && creature.CurrentHp <= creature.MaxHp / 4;
        if (low == _low)
            return;
        _low = low;
        if (low)
            Enter();
        else
            Exit();
    }

    public override async Task AfterCombatEnd(CombatRoom room)
    {
        if (_low) Exit();
    }

    public override async Task BeforeCombatStart()
    {
        if (_low) Enter();
    }

    static void Enter()
    {
        //var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, "Cavetickthreat.ogg");
        var path = "res://BirdMod/music/Cavetickthreat.ogg".SimplifyPath();
        // ReSharper disable once InconsistentNaming
        // var _track = AudioStreamOggVorbis.LoadFromFile("res://BirdMod/music/Cavetickthreat.ogg");
        var stream = GetOrLoadStream(path);
        var _track = stream;   
        _track.Loop = true; //cant do this anywhere else
        
        // the controller's StopMusic forgets the current track, so UpdateMusic restarts it on Exit
        NRunMusicController.Instance?.StopMusic();
        // game music (FMOD) ignores pause, so ours and its fades must too
        var player = new AudioStreamPlayer
        {
            Stream = _track,
            Bus = "Master",
            VolumeDb = SilenceDb,
            ProcessMode = Node.ProcessModeEnum.Always,
        };
        // run end frees NRun and the player with it. prevents blocking game music.
        player.TreeExiting += () =>
        {
            if (_player != player)
                return;
            _low = false;
            _player = null;
        };
        NRun.Instance!.AddChild(player);
        player.Play();
        Fade(player, TargetDb());
        _player = player;
    }

    static void Exit()
    {
        if (_player != null)
            Fade(_player, SilenceDb).Finished += _player.QueueFree;
        _player = null;
        var music = NRunMusicController.Instance;
        if (music != null)
        {
            // StopMusic in Enter stopped ambience without resetting how far the track got. we fix that one here.
            Traverse.Create(music).Field("_currentAmbience").SetValue(null);
            music.UpdateMusic();
            // UpdateMusic parks Progress at 0; the game follows it with the room's section
            music.UpdateTrack();
        }
    }

    static float TargetDb()
    {
        var s = SaveManager.Instance.SettingsSave;
        return Mathf.LinearToDb(Mathf.Pow(s.VolumeMaster * s.VolumeBgm, 2f));
    }

    static Tween Fade(AudioStreamPlayer player, float db)
    {
        var tween = player.CreateTween();
        tween.TweenProperty(player, "volume_db", db, FadeSeconds).SetTrans(Tween.TransitionType.Sine);
        return tween;
    }
    
    static AudioStreamOggVorbis? GetOrLoadStream(string file)
    {
        if (CachedStreams.TryGetValue(file, out AudioStreamOggVorbis value))
        {
            if (GodotObject.IsInstanceValid((GodotObject)(object)value))
            {
                return value;
            }
            CachedStreams.Remove(file);
        }
        AudioStreamOggVorbis val = GD.Load<AudioStreamOggVorbis>(file);
        if (val != null && val.GetLength() < 15.0)
        {
            CachedStreams[file] = val;
        }
        return val;
    }
}
*/