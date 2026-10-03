using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Chirp() : BirdModCard(2, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, ValueProp.Move),
        new PowerVar<DarkShacklesPower>(2M),
        new PowerVar<DexterityPower>(20M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StrengthPower>(),
            HoverTipFactory.FromPower<DexterityPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-CHIRP.flavor")),
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        var target = play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target];
        foreach (var guy in target)
            await CreatureCmd.LoseBlock(choiceContext, guy, guy.Block, oc);
        
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                .WithHitFx("vfx/vfx_attack_slash")
                .HelpTheCmd(oc, play, this, Owner)
                .Execute(choiceContext);
        await PowerCmd.Apply<ChirpDexPower>(choiceContext, target, this.DynamicVars["DexterityPower"].BaseValue, oc, (CardModel)this);
        await PowerCmd.Apply<ChirpStrPower>(choiceContext, target, this.DynamicVars["DarkShacklesPower"].BaseValue, oc, (CardModel)this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(6m);
        this.AddKeyword(CardKeyword.Retain);
    }
}