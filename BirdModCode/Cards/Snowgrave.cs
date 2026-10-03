using BaseLib.Audio;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace BirdMod.BirdModCode.Cards;

public class Snowgrave() : BirdModCard(-1,
    CardType.Attack, CardRarity.Ancient,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        VfxColor vfxColor = VfxColor.White;
        VfxDuration vfxDuration = VfxDuration.VeryLong;
        TalkCmd.Play(new LocString("cards", "BIRDMOD-SNOWGRAVE.talk"), oc, vfxColor, vfxDuration);
        ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/squeak.mp3"), 0f, 2f, 0.1f, 1f);
    }

    protected override void OnUpgrade()
    {

    }
}