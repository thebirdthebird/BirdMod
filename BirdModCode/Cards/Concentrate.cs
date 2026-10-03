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

public class Concentrate() : BirdModCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
        new CardsVar(1),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>  IsInCombat
        ?
        [
            this.EnergyHoverTip,
            HoverTipFactory.FromPower<EnergyNextTurnPower>(),
            HoverTipFactory.FromPower<DrawCardsNextTurnPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-CONCENTRATE.flavor")),
            this.EnergyHoverTip,
            HoverTipFactory.FromPower<EnergyNextTurnPower>(),
            HoverTipFactory.FromPower<DrawCardsNextTurnPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (Owner != null) await PlayerCmd.GainEnergy(this.DynamicVars.Energy.IntValue, this.Owner);
        if (Owner != null) await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, oc,
            base.DynamicVars.Energy.IntValue, oc, this);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, oc,
            base.DynamicVars.Cards.BaseValue, oc, this);

    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}