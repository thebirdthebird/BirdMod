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

namespace BirdMod.BirdModCode.Cards;

public class Overexert() : BirdModCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(3),
        new PowerVar<NoDrawPower>(1m)
        // new PowerVar<DrawLessNextTurnPower>(1m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            this.EnergyHoverTip,
            HoverTipFactory.FromPower<NoDrawPower>()
            // HoverTipFactory.FromPower<DrawLessNextTurnPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-OVEREXERT.flavor")),
        this.EnergyHoverTip,
        HoverTipFactory.FromPower<NoDrawPower>()
        // HoverTipFactory.FromPower<DrawLessNextTurnPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (Owner != null) await PlayerCmd.GainEnergy((Decimal) this.DynamicVars.Energy.IntValue, this.Owner);
        await PowerCmd.Apply<NoDrawPower>(choiceContext, oc, base.DynamicVars["NoDrawPower"].BaseValue, oc, this);
        // await PowerCmd.Apply<DrawLessNextTurnPower>(choiceContext, oc, base.DynamicVars["DrawLessNextTurnPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Energy.UpgradeValueBy(2m);
    }
}