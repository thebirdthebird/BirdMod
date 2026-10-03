using BaseLib.Audio;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;

namespace BirdMod.BirdModCode.Cards;

public class Ritual() : BirdModCard(2, CardType.Power,
    CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<RitualPower>(2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<RitualPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-RITUAL.flavor")),
        HoverTipFactory.FromPower<RitualPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        VfxColor vfxColor = VfxColor.White;
        VfxDuration vfxDuration = VfxDuration.VeryLong;
        await PowerCmd.Apply<RitualPower>(choiceContext, oc, base.DynamicVars["RitualPower"].BaseValue, oc, this);
        TalkCmd.Play(new LocString("cards", "BIRDMOD-RITUAL.talk" + Rng.Chaotic.NextInt(2)), oc, vfxColor, vfxDuration);
        ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/squeak.mp3"), 0f, 2f, 0.1f, 1f);
    }

    protected override void OnUpgrade()
    {
        //base.DynamicVars["RitualPower"].UpgradeValueBy(1m);
        this.AddKeyword(CardKeyword.Innate);
    }
}