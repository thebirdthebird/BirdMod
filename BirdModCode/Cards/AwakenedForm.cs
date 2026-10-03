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

public class AwakenedForm() : BirdModCard(3,
    CardType.Power, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<AwakenedFormPower>(2m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Ethereal
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<DexterityPower>()
        ]
        :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-AWAKENED_FORM.flavor")),
            HoverTipFactory.FromPower<DexterityPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature ownerCreature)
    {
        await PowerCmd.Apply<AwakenedFormPower>(choiceContext, ownerCreature, base.DynamicVars["AwakenedFormPower"].BaseValue, ownerCreature, this);
    }

    protected override void OnUpgrade()
    {
        // base.DynamicVars["AwakenedFormPower"].UpgradeValueBy(1m);
        this.RemoveKeyword(CardKeyword.Ethereal);
    }
}