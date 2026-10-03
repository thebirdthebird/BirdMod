using BirdMod.BirdModCode.Powers;
using BirdMod.BirdModCode.Relics;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Relics;

public class AerogelCalendar() : BirdModRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Uncommon;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<StaggeringPower>(52m),
        new IntVar("StaggeringHits", 21m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StaggeringPower>()
    ];
    
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(base.Owner.Creature) && base.Owner.PlayerCombatState!.TurnNumber <= 1)
        {
            Flash();
            var list = await PowerCmd.Apply<StaggeringPower>(choiceContext, combatState.HittableEnemies, this.DynamicVars["StaggeringPower"].BaseValue, base.Owner.Creature, null);
            foreach (var stagger in list)
            {
                stagger.SetHits(DynamicVars["StaggeringHits"].IntValue);
            }
        }
    }
    
}