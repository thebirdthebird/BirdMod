using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BirdMod.BirdModCode.Cards;


public class HarnessingEnergy() : BirdModCard(2,
    CardType.Power, CardRarity.Ancient,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<HarnessingEnergyPower>(1m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            this.EnergyHoverTip
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-HARNESSING_ENERGY.flavor")),
            this.EnergyHoverTip
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await PowerCmd.Apply<HarnessingEnergyPower>(choiceContext, oc, base.DynamicVars["HarnessingEnergyPower"].BaseValue, oc, null);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Innate);
    }
}