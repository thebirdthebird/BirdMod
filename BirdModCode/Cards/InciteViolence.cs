using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace BirdMod.BirdModCode.Cards;

public class InciteViolence() : BirdModCard(1,
    CardType.Skill, CardRarity.Uncommon,
    TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new CardsVar(1),
        new IntVar("Adroit", 3m),
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromCard<Talon>(),
            .. HoverTipFactory.FromEnchantment<Adroit>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-INCITE_VIOLENCE.flavor")),
            HoverTipFactory.FromCard<Talon>(),
            .. HoverTipFactory.FromEnchantment<Adroit>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (base.CombatState == null) return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(oc)
            where c != null && c.IsAlive && c.IsPlayer
            select c;
        foreach (Creature creature in enumerable)
        {
            List<Talon> cards = new List<Talon>();
            for (int i = 0; i < DynamicVars.Cards.BaseValue; i++)
            {
                cards.Add(CombatState.CreateCard<Talon>(creature.Player!));
            }
            IReadOnlyList<CardPileAddResult> results = await CardPileCmd.AddGeneratedCardsToCombat(cards, PileType.Draw, base.Owner, CardPilePosition.Random);
            foreach (var item in results)
            {
                CardCmd.Enchant<Adroit>(item.cardAdded, DynamicVars["Adroit"].IntValue);
            }
            if (LocalContext.IsMe(creature))
            {
                CardCmd.PreviewCardPileAdd(results);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Cards.UpgradeValueBy(1m);
        base.DynamicVars["Adroit"].UpgradeValueBy(1m);
    }
}