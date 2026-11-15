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

public class Flirt() : BirdModCard(2, CardType.Skill,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // new PowerVar<StrengthPower>(4m),
        new PowerVar<WeakPower>(5m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            // HoverTipFactory.FromPower<StrengthPower>()
            HoverTipFactory.FromPower<WeakPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-FLIRT.flavor")),
        // HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        // await PowerCmd.Apply<StrengthPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], -this.DynamicVars["StrengthPower"].BaseValue, oc, (CardModel) this);
        await PowerCmd.Apply<WeakPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], this.DynamicVars["WeakPower"].BaseValue, oc, (CardModel) this);

    }

    protected override void OnUpgrade()
    {
        // base.DynamicVars["StrengthPower"].UpgradeValueBy(2m);
        DynamicVars["WeakPower"].UpgradeValueBy(3m);
    }
}