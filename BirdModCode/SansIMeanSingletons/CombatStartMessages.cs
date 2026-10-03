using BaseLib.Abstracts;
using BaseLib.Audio;
using BirdMod.BirdModCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace BirdMod.BirdModCode.SansIMeanSingletons;

public class CombatStartMessages() : CustomSingletonModel(HookType.Combat)
{
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!BirdModConfig.ChattyBird) { MainFile.Logger.Info("[BIRDMOD] Skipping bird talking, user has it set to false. what a shame..."); return; }
        if (combatState.RoundNumber > 1 || (int)side != 1) return;
        if (combatState.Encounter == null) return;
        var player = combatState.Players.FirstOrDefault(p => p.Character is Character.BirdMod);
        if (player == null) return;
        var a = new LocString("battle", "BIRDMOD-" + combatState.Encounter.Id.Entry);
        var flag = a.GetRawText() == "[outline_size=8][outline_color=white][color=#000000]Placeholder.[/color][/outline_color][/outline_size]";
        if (a.Exists() && !flag)
        {
            TalkCmd.Play(a, player.Creature, VfxColor.White, VfxDuration.VeryLong);
            ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/squeak.mp3"), 0f, 2f, 0.1f, 1f);
        }
    }
}