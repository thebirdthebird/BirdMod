using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

  
public class Flatter() : BirdModCard(2,
    CardType.Skill, CardRarity.Common,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(10, ValueProp.Move),
        new PowerVar<DarkShacklesPower>(2m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StrengthPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-FLATTER.flavor")),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await CreatureCmd.GainBlock(oc, base.DynamicVars.Block, play);
        if (play != null)
            if (play.Target == null)
                return;
        await PowerCmd.Apply<FlatterPower>(choiceContext, play?.Target  == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], this.DynamicVars["DarkShacklesPower"].BaseValue, oc, (CardModel)this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(2m);
        base.DynamicVars["DarkShacklesPower"].UpgradeValueBy(1m);
    }
}