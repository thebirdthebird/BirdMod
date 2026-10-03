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

namespace BirdMod.BirdModCode.Cards;

public class Hunt() : BirdModCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<WeakPower>(1m),
        new PowerVar<VulnerablePower>(1m),
        new PowerVar<DexterityPower>(10m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<VulnerablePower>(),
            HoverTipFactory.FromPower<DexterityPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-HUNT.flavor")),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        if (oc.CombatState == null) return;
        await PowerCmd.Apply<VulnerablePower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], this.DynamicVars["VulnerablePower"].BaseValue, oc, (CardModel)this);
        await PowerCmd.Apply<WeakPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], this.DynamicVars["WeakPower"].BaseValue, oc, (CardModel)this);
        await PowerCmd.Apply<DexterityPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], -this.DynamicVars["DexterityPower"].BaseValue, oc, (CardModel)this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["WeakPower"].UpgradeValueBy(1m);
        base.DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
        base.DynamicVars["DexterityPower"].UpgradeValueBy(8m);
    }
}