using BaseLib.Audio;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace BirdMod.BirdModCode.Cards;

public class PlatonicKiss() : BirdModCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AnyAlly)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
        new CardsVar(1),
        new PowerVar<PlatonicKissPower>(4m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            this.EnergyHoverTip,
            HoverTipFactory.FromPower<DexterityPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-PLATONIC_KISS.flavor")),
            this.EnergyHoverTip,
            HoverTipFactory.FromPower<DexterityPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play.Target == null) return;
        if (play.Target.Player == null) return;
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, play.Target.Player);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, play.Target.Player);
        await PowerCmd.Apply<PlatonicKissPower>(choiceContext, play.Target, base.DynamicVars["PlatonicKissPower"].BaseValue, oc, this);
        TalkCmd.Play(new LocString("cards", "BIRDMOD-PLATONIC_KISS.talk"), oc, VfxColor.White, VfxDuration.VeryLong);
        ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/squeak.mp3"), 0f, 2f, 0.1f, 1f);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars["PlatonicKissPower"].UpgradeValueBy(3m);
    }
}