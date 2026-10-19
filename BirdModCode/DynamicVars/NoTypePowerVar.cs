using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BirdMod.BirdModCode.DynamicVars;

public class NoTypePowerVar : DynamicVar
{
    public PowerModel PowerModelVar;
    
    public NoTypePowerVar(PowerModel powerModel, decimal powerAmount)
        : base(powerModel.GetType().Name, powerAmount)
    {
        PowerModelVar = powerModel;
    }

    public NoTypePowerVar(PowerModel powerModel, string name, decimal powerAmount)
        : base(name, powerAmount)
    {
        PowerModelVar = powerModel;
    }
    
    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        if (runGlobalHooks)
        {
            base.PreviewValue = Hook.ModifyPowerAmountGiven(card.CombatState, PowerModelVar, card.Owner.Creature, base.BaseValue, target, card, out IEnumerable<AbstractModel> _);
        }
    }
}
