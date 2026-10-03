using BaseLib.Cards.Variables;
using BaseLib.Commands;
using BaseLib.Extensions;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Cards;

public class Research() : BirdModCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new ScryVar(3m),
        new CardsVar(2),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            // HoverTipFactory.FromPower<DexterityPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-RESEARCH.flavor")),
        // HoverTipFactory.FromPower<DexterityPower>()
    ];
    
    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (Owner == null) return; // NO! STOP IT
        await ScryCmd.Execute(choiceContext, this);
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
    }

    protected override void OnUpgrade()
    {
        // base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars.Scry().UpgradeValueBy(3m);
        // base.DynamicVars["DexterityPower"].UpgradeValueBy(2m);
    }
}