using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Cards;

public class Foreordain() : BirdModCard(0,
    CardType.Skill, CardRarity.Rare,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<ForeordainPower>(3m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust,
        CardKeyword.Retain
    ];
    
    /*
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<DodgePower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-FOREORDAIN.flavor")),
            HoverTipFactory.FromPower<DodgePower>()
        ];
        */

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> htlist = new List<IHoverTip>();
            if (!IsInCombat)
            {
                htlist.Add(new HoverTip(new LocString("cards", "BIRDMOD-FOREORDAIN.flavor")));
            }
            if (IsUpgraded)
            {
                htlist.Add(HoverTipFactory.FromPower<IntangiblePower>());
            }
            htlist.Add(HoverTipFactory.FromPower<DodgePower>());
            
            
            return ((IEnumerable<IHoverTip>) htlist);
        }
    }

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (IsUpgraded)
        {
            await PowerCmd.Apply<UpgradedForeordainPower>(choiceContext, oc, base.DynamicVars["ForeordainPower"].BaseValue, oc, this);
            await PowerCmd.Apply<ForeordainPower>(choiceContext, oc, base.DynamicVars["ForeordainPower"].BaseValue, oc, this);
        }
        else
        {
            await PowerCmd.Apply<ForeordainPower>(choiceContext, oc, base.DynamicVars["ForeordainPower"].BaseValue, oc, this);
        }
    }

    protected override void OnUpgrade()
    {
        
    }
}