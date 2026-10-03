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

public class StaggeringPotion : BirdModPotion
{
    public override PotionRarity Rarity => PotionRarity.Uncommon;

    public override PotionUsage Usage => PotionUsage.CombatOnly;

    public override TargetType TargetType => TargetType.AnyEnemy;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StaggeringPower>(40m),
        new IntVar("StaggeringHits", 4m)
    ];
    
    public override IEnumerable<IHoverTip> ExtraHoverTips => 
    [
        new HoverTip(new LocString("potions", "BIRDMOD-STAGGERING_POTION.flavor")),
        HoverTipFactory.FromPower<StaggeringPower>()
    ];

    protected override async Task OnUse(PlayerChoiceContext choiceContext, Creature? target)
    {
        PotionModel.AssertValidForTargetedPotion(target);
        StaggeringPower? staggering = await PowerCmd.Apply<StaggeringPower>(choiceContext, target, this.DynamicVars["StaggeringPower"].BaseValue, this.Owner.Creature, null, false);
        if (staggering != null)
        {
            staggering.SetHits(this.DynamicVars["StaggeringHits"].IntValue);
        }
    }
}