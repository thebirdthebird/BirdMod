using BaseLib.Abstracts;
using BaseLib.Audio;
using BaseLib.Extensions;
using BirdMod.BirdModCode.Utils;
using Godot;
using GodotPlugins.Game;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Rooms;

namespace BirdMod.BirdModCode.SansIMeanSingletons;

// at <= 1/4 hp the act music drops out and Cavetickthreat loops until you heal back up.
// Run and not Combat: a Combat singleton is only hooked during a combat, so it never sees a
// campfire/potion/event heal.
[HarmonyPatch]
public class Music() : CustomSingletonModel(HookType.Combat)
{
    const float SilenceDb = -80f;
    const float FadeSeconds = 1.5f;

    static readonly ModSound ThreatTrack =
        new("res://BirdMod/music/Cavetickthreat.ogg", ModAudio.SoundType.Music);
    
    static readonly ModSound JankTrack =
        new("res://BirdMod/music/doomapproaches.ogg", ModAudio.SoundType.Music);

    static AudioStreamPlayer? _player;
    static AudioStreamPlayer? _fading;
    static Tween? _fadeOut;
    static bool _low;
    static ulong _runId;

    // gated on the player and not on _low, otherwise Exit's own resume gets blocked
    static bool Playing
    {
        get
        {
            if (_player != null && !GodotObject.IsInstanceValid(_player))
                _player = null; // run ended and freed it with NRun
            return _player != null;
        }
    }

    // while our track is up the run doesn't get to start music of its own (new room, boss)
    [HarmonyPrefix]
    [HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.UpdateMusic))]
    static bool BlockUpdateMusic() => !Playing;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NRunMusicController), nameof(NRunMusicController.PlayCustomMusic))]
    static bool BlockCustomMusic() => !Playing;

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        if (!LocalContext.IsMe(creature) || NRun.Instance == null || !BirdModConfig.CloseToDeathMusic || creature.Player == null)
            return Task.CompletedTask;
        var p = creature.Player.Character is Character.BirdMod;
        if (!p) return Task.CompletedTask;

        SyncRun();

        var low = creature.CurrentHp > 0 && creature.CurrentHp <= creature.MaxHp / 4;
        if (low == _low)
            return Task.CompletedTask;

        _low = low;
        if (low)
            Enter(ThreatTrack);
        else
            // at 0 hp the death jingle owns the speakers. IsDead flips later than this hook, so read the hp
            Exit(resumeGameMusic: creature.CurrentHp > 0);
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        var flag = false;
        var low = false;
        if (combatState.Encounter == null) return Task.CompletedTask;
        if (combatState.RoundNumber > 1 || (int)side != 1) return Task.CompletedTask;
        var name = combatState.Encounter.Id.Entry;
        MainFile.Logger.Info("hey by the way this is encounter " + name);
        if (name.Contains("BIRDMOD"))
        {
            // hey this means it's one of my encounters and i am now
            // hijacking this lowhp thing for music for my encounters
            // put some code here play some funky music
            flag = true;
            MainFile.Logger.Info("WE HAVE SET FLAG TO TRUE");
        }
        var player = combatState.Players.FirstOrDefault(p => p.Character is Character.BirdMod);
        if (player == null)
        {
            // NOT A BIRD
            if (flag) // IF IN BIRD COMBAT, JUST SET LOW TO FALSE
            {
                low = false;
            }
            else // NOT BIRD NOR IN BIRD COMBAT... RETREAT!!!
            {
                return Task.CompletedTask; // die here
            }
        }
        else // YOU ARE, IN FACT , A BIRD
        {
            // regardless of flag, set low... you want low hp music even in 
            low = player.Creature.CurrentHp > 0 && player.Creature.CurrentHp <= player.Creature.MaxHp / 4;
        }
        SyncRun();
        _low = low;
        MainFile.Logger.Info("WE HAVE SET LOW TO " + low);
        if (low)
        {
            MainFile.Logger.Info("Entering...");
            Enter(ThreatTrack);
        }
        else
        {
            if (flag)
            {
                MainFile.Logger.Info("PLAY JANK...");
                Enter(JankTrack);
            }
            else
            {
                MainFile.Logger.Info("Exit...");
                if (player == null) return Task.CompletedTask;
                Exit(resumeGameMusic: player.Creature.CurrentHp > 0);
            }
        }
        return Task.CompletedTask;
    }

    /*
    public override Task BeforeCombatStart()
    {
        SyncRun();
        if (_low)
            Enter();
        return Task.CompletedTask;
    }
    */

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Exit(resumeGameMusic: true);
        return Task.CompletedTask;
    }

    // new NRun means new run, so the old _low is stale. has to be on whichever hook lands first:
    // from BeforeCombatStart alone it would wipe a _low set by neow/event damage and never exit.
    static void SyncRun()
    {
        var run = NRun.Instance;
        if (run == null || run.GetInstanceId() == _runId)
            return;
        _runId = run.GetInstanceId();
        _low = false;
    }

    static void Enter(ModSound trackToPlay)
    {
        if (Playing)
            return;

        // whipping across the threshold can land here mid fade-out. that player is still in the
        // tree under the track's name, and ModAudio refuses to start a track it thinks is playing
        DropFadingPlayer();

        SilenceGameMusic();

        var player = ModAudio.PlaySoundInRun(trackToPlay, 0f, 0.6f);
        if (player == null)
        {
            // master or bgm at zero, or BaseLib's two music players are taken
            ResumeGameMusic();
            return;
        }

        // game music (FMOD) ignores pause, so ours must too
        player.ProcessMode = Node.ProcessModeEnum.Always;
        if (player.Stream is AudioStreamOggVorbis ogg)
            ogg.Loop = true; //cant do this anywhere else
        player.FadeIn(FadeSeconds);
        _player = player;
    }

    static void Exit(bool resumeGameMusic)
    {
        if (!Playing)
            return;

        var player = _player!;
        _player = null; // before the resume, or BlockUpdateMusic swallows it

        DropFadingPlayer();
        AudioStreamPlayerExtensions.CurrentTween[player]?.Kill(); // FadeIn, if it is still running
        _fading = player;
        _fadeOut = Fade(player, SilenceDb);
        _fadeOut.Finished += () =>
        {
            if (_fading == player)
                DropFadingPlayer();
        };

        if (resumeGameMusic)
            ResumeGameMusic();
    }

    // hand the player back to ModAudio's pool. not QueueFree: the pool owns it
    static void DropFadingPlayer()
    {
        _fadeOut?.Kill();
        _fadeOut = null;
        if (_fading != null && GodotObject.IsInstanceValid(_fading))
            _fading.GetParent()?.RemoveChild(_fading);
        _fading = null;
    }

    static Tween Fade(AudioStreamPlayer player, float db)
    {
        var tween = player.CreateTween();
        tween.TweenProperty(player, "volume_db", db, FadeSeconds).SetTrans(Tween.TransitionType.Sine);
        return tween;
    }

    static void SilenceGameMusic()
    {
        if (NRunMusicController.Instance is not { } music)
            return;

        // not StopMusic(): it unloads the act bank and forgets _currentTrack, so there is
        // nothing left for StopCustomMusic to bring back
        var proxy = Traverse.Create(music).Field<Node>("_proxy").Value;
        proxy.Call("stop_music");
        proxy.Call("stop_ambience");
    }

    static void ResumeGameMusic()
    {
        if (NRunMusicController.Instance is not { } music)
            return;

        music.StopCustomMusic();
        // we stopped ambience behind the controller's back; clearing the field makes it reload
        Traverse.Create(music).Field("_currentAmbience").SetValue(null);
        music.UpdateAmbience();
        // StopCustomMusic parks Progress at 7 (CombatEnd); put it back on the room's own section
        music.UpdateTrack();
    }
}