using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Cards;

[Pool(typeof(TokenCardPool))]
public class Urge() : CallCard(-1, CardType.Skill,
    CardRarity.Token, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(1),
        new CardsVar(2),
        new PowerVar<DrawLessNextTurnPower>(1m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            this.EnergyHoverTip,
            HoverTipFactory.FromPower<DrawLessNextTurnPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-URGE.flavor")),
        this.EnergyHoverTip,
        HoverTipFactory.FromPower<DrawLessNextTurnPower>()
    ];

    public override async Task OnAncestralCall(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (Owner != null) await PlayerCmd.GainEnergy((Decimal) this.DynamicVars.Energy.IntValue, this.Owner);
        if (Owner != null) await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        await PowerCmd.Apply<DrawLessNextTurnPower>(choiceContext, oc, base.DynamicVars["DrawLessNextTurnPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}