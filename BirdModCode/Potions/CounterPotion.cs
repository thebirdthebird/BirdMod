using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BirdMod.BirdModCode.Potions;

public class CounterPotion : BirdModPotion
{
    public override PotionRarity Rarity => PotionRarity.Common;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyPlayer;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<CounterPower>(25m)
    ];
    
    public override IEnumerable<IHoverTip> ExtraHoverTips => 
        [
            new HoverTip(new LocString("potions", "BIRDMOD-COUNTER_POTION.flavor")),
            HoverTipFactory.FromPower<CounterPower>()
        ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);
        await PowerCmd.Apply<CounterPower>(choiceContext, target, base.DynamicVars["CounterPower"].BaseValue, this.Owner.Creature, null);
    }
}