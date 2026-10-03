using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

[Pool(typeof(QuestCardPool))]
public class Imbue() : BirdModCard(0,
    CardType.Quest, CardRarity.Quest,
    TargetType.None)
{
    public const int MaxEnergy = 15;

    private const string _energyKey = "Energy";

    private int _energy;
    
    protected override bool HasEnergyCostX => true;
    
    public override bool CanBeGeneratedInCombat => false;
    public override int MaxUpgradeLevel => 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Energy", 15m)
    ];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        CardKeyword.Retain,
        CardKeyword.Exhaust
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            this.EnergyHoverTip
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-IMBUE.flavor")),
            this.EnergyHoverTip
        ];
    
    [SavedProperty]
    public int Energy
    {
        get => _energy;
        set
        {
            AssertMutable();
            _energy = value;
            base.DynamicVars["Energy"].BaseValue = MaxEnergy - Energy;
        }
    }

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        int num = ResolveEnergyXValue();
        if (num > 0)
        {
            for (int i = 0; i < num; ++i)
            {
                if (Energy + 1 < MaxEnergy + 1)
                {
                    if (base.DeckVersion is Imbue deckImbue) deckImbue.Energy++;
                    Energy++;
                }
            }
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {   
        if (Energy >= MaxEnergy)
        {
            if (base.DeckVersion is Imbue)
            {
                PlayerCmd.CompleteQuest(base.DeckVersion);
                CardCmd.TransformTo<HarnessingEnergy>(base.DeckVersion); //temp
            }
        }
        return base.AfterCombatEnd(room);
    }

    protected override void OnUpgrade()
    {
        
    }
}