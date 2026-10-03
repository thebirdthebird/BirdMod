using BaseLib.Abstracts;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BirdMod.BirdModCode.SansIMeanSingletons;

public class TalonScaling() : CustomSingletonModel(HookType.Combat)
{
    private static int _amountOfTimesScaled = 0;
    
    public static void ScaleDamage(int heyHowMuchAreWeScaling, PlayerCombatState? pcsThatIHate)
    {
        _amountOfTimesScaled += heyHowMuchAreWeScaling;
        // MainFile.Logger.Info($"We Scaled {heyHowMuchAreWeScaling} times");
        if (pcsThatIHate == null) return;
        
        IEnumerable<Talon> enumerable = pcsThatIHate.AllCards.OfType<Talon>();
        decimal baseValue = heyHowMuchAreWeScaling;
        foreach (Talon item in enumerable)
        {
            item.BuffFromTalonPlay(baseValue);
        }
        IEnumerable<TalonOutburst> enumerable2 = pcsThatIHate.AllCards.OfType<TalonOutburst>();
        foreach (TalonOutburst item2 in enumerable2)
        {
            item2.BuffFromTalonPlay(baseValue);
        }
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (card is Talon talon)
        {
            talon.BuffFromTalonPlay(_amountOfTimesScaled);
        }

        if (card is TalonOutburst talonOutburst)
        {
            talonOutburst.BuffFromTalonPlay(_amountOfTimesScaled);
        }
            
        return base.AfterCardGeneratedForCombat(card, creator);
    }
    
    public override async Task AfterCombatEnd(CombatRoom _)
    {
        _amountOfTimesScaled = 0;
    }

    public static int GetAmountOfTimesScaled() => _amountOfTimesScaled;
    
}