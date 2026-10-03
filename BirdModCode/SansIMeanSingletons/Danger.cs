using BaseLib.Abstracts;
using BaseLib.Audio;
using BirdMod.BirdModCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.SansIMeanSingletons;

public class Danger() : CustomSingletonModel(HookType.Combat)
{
    /*
    private static bool isPlaying = false;
    
    private NRunMusicController? _music = NRunMusicController.Instance;

    private string _currentMusic = "res://BirdMod/music/Cavetickthreat.ogg";
    
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext,
        Creature target,
        DamageResult result,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource)
    {
        if (!target.IsPlayer || target.Player is not { Character: Character.BirdMod } player) return;
        if (target.CurrentHp <= 0) return;
        if (!BirdModConfig.CloseToDeathMusic) return;
        
        if (target.CurrentHp <= target.MaxHp / 4)
        {
            _music?.StopMusic(); //straight up does nothing
            _music?.PlayCustomMusic(_currentMusic); //also does nothing
            _music?.UpdateTrack(); //still does nothing
            //var snd = new ModSound(_currentMusic, ModAudio.SoundType.Music); //works but doesnt pause vanilla music and doesnt loop
            //ModAudio.PlaySound(snd); //see above
            MainFile.Logger.Info("[BIRDMOD] WE ARE TRYING TO PLAY CLOSE TO DEATH MUSIC!!!!");
            isPlaying = true;
        }
        else
        {
            
            isPlaying = false;
            // how do I stop the Sound if it's currently playing?
            MainFile.Logger.Info("[BIRDMOD] WE ARE TRYING TO STOPPPPP CLOSE TO DEATH MUSIC!!!!");
            _music?.StopCustomMusic();
            _music?.UpdateTrack();
        }
    }
    
    */
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        var player = combatState.Players.FirstOrDefault(p => p.Character is Character.BirdMod);
        if (player == null) return;
        if (player.Creature.IsDead) return;
        if (player.Creature.CurrentHp > player.Creature.MaxHp / 4) return;
        
        if ((int)side != 1) return;
        if (!BirdModConfig.ChattyBird) { MainFile.Logger.Info("[BIRDMOD] Skipping bird talking, user has it set to false. what a shame..."); return; }
        if (combatState.Encounter == null) return;
        
        
        var a = new LocString("battle", "BIRDMOD-low." + Rng.Chaotic.NextInt(5));
        var flag = a.GetRawText() == "[outline_size=8][outline_color=white][color=#000000]Placeholder.[/color][/outline_color][/outline_size]";
        if (a.Exists() && !flag)
        {
            TalkCmd.Play(a, player.Creature, VfxColor.White, VfxDuration.VeryLong);
            ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/squeak.mp3"), 0f, 2f, 0.1f, 1.2f);
        }
        
    }
}