using BaseLib.Audio;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace BirdMod.BirdModCode.Cards;

public class Flirt() : BirdModCard(2, CardType.Power,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    
    /*
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    */
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StrengthPower>(4m),
        // new PowerVar<WeakPower>(5m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StrengthPower>()
            // HoverTipFactory.FromPower<WeakPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-FLIRT.flavor")),
        HoverTipFactory.FromPower<StrengthPower>(),
        // HoverTipFactory.FromPower<WeakPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        // await PowerCmd.Apply<StrengthPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], -this.DynamicVars["StrengthPower"].BaseValue, oc, (CardModel) this);
        await PowerCmd.Apply<StrengthPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], this.DynamicVars["StrengthPower"].BaseValue, oc, (CardModel) this);
        if (play?.Target != null)
        {
            TalkCmd.Play(new LocString("cards", "BIRDMOD-FLIRT.talk"), oc, VfxColor.White, VfxDuration.VeryLong);
            ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/noise.mp3"), 0f, 2f, 0.1f, 1f);
            await CreatureCmd.Stun(play.Target);
        }

    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["StrengthPower"].UpgradeValueBy(-2m);
        // DynamicVars["WeakPower"].UpgradeValueBy(3m);
    }
}