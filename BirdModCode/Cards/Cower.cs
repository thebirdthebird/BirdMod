using BaseLib.Cards.Variables;
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


public class Cower() : BirdModCard(2, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(20m, ValueProp.Move),
        new PowerVar<WeakPower>(2),
        new PowerVar<DrawCardsNextTurnPower>(2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<DrawCardsNextTurnPower>(),
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-COWER.flavor")),
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<DrawCardsNextTurnPower>(),
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await CreatureCmd.GainBlock(oc, DynamicVars.Block, play);
        await PowerCmd.Apply<WeakPower>(choiceContext, oc, base.DynamicVars["WeakPower"].BaseValue, oc, this);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, oc, base.DynamicVars["DrawCardsNextTurnPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(5m);
    }
}