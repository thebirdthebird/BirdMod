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
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;


public class Ram() : BirdModCard(2, CardType.Skill,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(13, ValueProp.Move),
        new IntVar("StaggeringHits", 5m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>(),
            HoverTipFactory.FromCard<Stumble>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-SLIDE.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>(),
            HoverTipFactory.FromCard<Stumble>()
        ];
    
    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await CreatureCmd.GainBlock(oc, base.DynamicVars.Block, play);
        if (play != null)
            if (play.Target == null)
                return;
        var staggering = await PowerCmd.Apply<StaggeringPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], oc.Block, oc, this, false);
        foreach (var pain in staggering)
        {
            pain.SetHits(this.DynamicVars["StaggeringHits"].IntValue);
        }
        if (Owner == null) return; // die
        CardModel card = this.CombatState!.CreateCard<Stumble>(this.Owner);
        List<CardModel> cards = new List<CardModel>();
        for (int index = 0; index < 1; ++index)
            cards.Add(card);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, this.Owner, CardPilePosition.Random));
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(5m);
    }
}