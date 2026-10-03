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

public class Retaliate() : BirdModCard(1,
    CardType.Power, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ThornsPower>(1m),
        new PowerVar<CounterPower>(9m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<ThornsPower>(),
            HoverTipFactory.FromPower<CounterPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-RETALIATE.flavor")),
        HoverTipFactory.FromPower<ThornsPower>(),
        HoverTipFactory.FromPower<CounterPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await PowerCmd.Apply<ThornsPower>(choiceContext, oc, base.DynamicVars["ThornsPower"].BaseValue, oc, this);
        await PowerCmd.Apply<CounterPower>(choiceContext, oc, base.DynamicVars["CounterPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["ThornsPower"].UpgradeValueBy(1m);
        base.DynamicVars["CounterPower"].UpgradeValueBy(5m);
    }
}