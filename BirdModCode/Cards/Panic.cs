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

public class Panic() : BirdModCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(13, ValueProp.Move),
        new PowerVar<DexterityPower>(2m),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<DexterityPower>(),
            HoverTipFactory.FromCard<Stumble>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-PANIC.flavor")),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromCard<Stumble>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await CreatureCmd.GainBlock(oc, base.DynamicVars.Block, play);
        await PowerCmd.Apply<PanicPower>(choiceContext, oc, base.DynamicVars["DexterityPower"].BaseValue, oc, this);
        if (Owner == null) return;
        CardModel card = this.CombatState!.CreateCard<Stumble>(this.Owner);
        List<CardModel> cards = new List<CardModel>();
        for (int index = 0; index < 1; ++index)
            cards.Add(card);
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Discard, this.Owner));
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(4m);
        base.DynamicVars["DexterityPower"].UpgradeValueBy(1m);
    }
}