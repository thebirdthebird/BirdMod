using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Audio;
using BaseLib.Extensions;
using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BirdMod.BirdModCode.Cards;

[Pool(typeof(TokenCardPool))]
public class AncestralGuidance() : BirdModCard(0,
    CardType.Skill, CardRarity.Ancient,
    TargetType.Self)
{
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromCard<Warn>(IsUpgraded),
            HoverTipFactory.FromCard<Encourage>(IsUpgraded),
            HoverTipFactory.FromCard<Urge>(IsUpgraded)
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-ANCESTRAL_GUIDANCE.flavor")),
        HoverTipFactory.FromCard<Warn>(IsUpgraded),
        HoverTipFactory.FromCard<Encourage>(IsUpgraded),
        HoverTipFactory.FromCard<Urge>(IsUpgraded)
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature ownerCreature)
    {
        VfxColor vfxColor = VfxColor.White;
        VfxDuration vfxDuration = VfxDuration.VeryLong;
        
        if (Owner.Creature.CombatState == null) return;
        CardModel card1 = Owner.Creature.CombatState.CreateCard<Warn>(Owner);
        CardModel card2 = Owner.Creature.CombatState.CreateCard<Encourage>(Owner);
        CardModel card3 = Owner.Creature.CombatState.CreateCard<Urge>(Owner);

        if (IsUpgraded)
        {
            CardCmd.Upgrade(card1);
            CardCmd.Upgrade(card2);
            CardCmd.Upgrade(card3);
        }
        
        List<CardModel> list = [
            card1,
            card2,
            card3
        ];
        
        CardModel? cardChosen = await CardSelectCmd.FromChooseACardScreen(choiceContext, (IReadOnlyList<CardModel>) list, this.Owner, false);
        if (cardChosen == null)
            return;
        if (cardChosen is CallCard realCard) 
            await realCard.OnAncestralCall(choiceContext, play, ownerCreature);
        
        RitualFeather? ritualFeather = this.Owner.GetRelic<RitualFeather>();
        if (ritualFeather == null) return;
        int num = ritualFeather.TimesAncestralGuidancePlayed;
        if (num >= 3)
            num = 3;
        
        TalkCmd.Play(new LocString("cards", "BIRDMOD-ANCESTRAL_GUIDANCE.talk" + num), this.Owner.Creature, vfxColor, vfxDuration);
        ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/squeak.mp3"), 0f, 2f, 0.1f, 1f);
    }

    protected override void OnUpgrade()
    {
        
    }
}