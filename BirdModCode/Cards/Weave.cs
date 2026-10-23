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
        // new PowerVar<DodgePower>(1M)
        new BlockVar(12m, ValueProp.Move),
        new PowerVar<VimPower>(2m)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<VimPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-WEAVE.flavor")),
       HoverTipFactory.FromPower<VimPower>()
    ];
    
    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await CreatureCmd.GainBlock(oc, DynamicVars.Block, play);
        await PowerCmd.Apply<VimPower>(choiceContext, oc, base.DynamicVars["VimPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        // this.AddKeyword(CardKeyword.Retain);
        DynamicVars.Block.UpgradeValueBy(4m);
        DynamicVars["VimPower"].UpgradeValueBy(1m);
    }
}