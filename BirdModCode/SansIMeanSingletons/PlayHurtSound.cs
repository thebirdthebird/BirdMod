using BaseLib.Abstracts;
using BaseLib.Audio;
using BaseLib.Utils;
using BirdMod.BirdModCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.SansIMeanSingletons;

public class PlayHurtSound() : CustomSingletonModel(HookType.Combat)
{
    private Creature? trackTheDealer = null;
    
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, 
        Creature target, 
        DamageResult result, 
        ValueProp props,
        Creature? dealer, 
        CardModel? cardSource)
    {
        if (!target.IsPlayer || target.Player is not { Character: Character.BirdMod } player) return;
        if (target != dealer && props.IsPoweredAttack() && result.UnblockedDamage > 0)
        {
            VfxColor vfxColor = VfxColor.White;
            VfxDuration vfxDuration = VfxDuration.VeryLong;
            
            ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/ow.ogg"), 0f, 3f, 0.1f, 1f);
            
            if (!BirdModConfig.ChattyBird)
            {
                MainFile.Logger.Info("[BIRDMOD] Skipping bird talking, user has it set to false. what a shame...");
                return;
            }
            
            if (trackTheDealer != dealer && target.IsAlive) 
            {
                TalkCmd.Play(OwieText(result, target), target.Player.Creature, vfxColor, vfxDuration);
                trackTheDealer = dealer;
            }
        }
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        trackTheDealer = null;
    }

    private LocString OwieText(DamageResult result, Creature target)
    {
        bool big = result.UnblockedDamage >= target.MaxHp/4;
        bool uhoh = target.CurrentHp <= target.MaxHp/4;
        var act = target.Player!.RunState.CurrentActIndex;
        if (act > 2) act = 2; // 3rd act is the end of the game, if you're higher than that, please stop
        // BIRDMOD-0.big.2 is BIRDMOD- (Act1) . (bigdamage?) . (random)
        if (uhoh) return new LocString("battle", "BIRDMOD-low." + Rng.Chaotic.NextInt(5));
        return new LocString("owie", "BIRDMOD-" + act + "." + (big ? "big" : "small") + "." + Rng.Chaotic.NextInt(5));
    }
}