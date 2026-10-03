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


public class AncestralProtection() : BirdModCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AncestralProtectionPower>(2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<CounterPower>()
        ]
        :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-ANCESTRAL_PROTECTION.flavor")),
            HoverTipFactory.FromPower<CounterPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature ownerCreature)
    {
        await PowerCmd.Apply<AncestralProtectionPower>(choiceContext, ownerCreature, base.DynamicVars["AncestralProtectionPower"].BaseValue, ownerCreature, this);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}