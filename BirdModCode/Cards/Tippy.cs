using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
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

public class Tippy() : BirdModCard(1, CardType.Attack,
    CardRarity.Common, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move),
        new CardsVar(1)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            ..HoverTipFactory.FromCardWithCardHoverTips<Tappy>(IsUpgraded)
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-TIPPY.flavor")),
        ..HoverTipFactory.FromCardWithCardHoverTips<Tappy>(IsUpgraded)
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                .WithHitFx("vfx/vfx_attack_slash")
                .HelpTheCmd(oc, play, this, Owner)
                .Execute(choiceContext);
        if (Owner == null) return;
        CardModel card = this.CombatState!.CreateCard<Tappy>(this.Owner);
        if (this.IsUpgraded)
            CardCmd.Upgrade(card);
        List<CardModel> cards = new List<CardModel>();
        for (int index = 0; index < this.DynamicVars.Cards.IntValue; ++index)
            cards.Add(card);
        await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Hand, this.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
    }
}