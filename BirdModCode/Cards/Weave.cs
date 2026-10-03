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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Weave() : BirdModCard(2,
    CardType.Skill, CardRarity.Basic,
    TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<DodgePower>(1M)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<DodgePower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-WEAVE.flavor")),
       HoverTipFactory.FromPower<DodgePower>()
    ];
    
    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await PowerCmd.Apply<DodgePower>(choiceContext, oc, base.DynamicVars["DodgePower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}