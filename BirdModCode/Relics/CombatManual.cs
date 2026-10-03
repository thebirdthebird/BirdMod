using BirdMod.BirdModCode.Powers;
using BirdMod.BirdModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Relics;

public class CombatManual() : BirdModRelic
{
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<DodgePower>(1m)
    ];
    
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, 
        Creature target, 
        DamageResult result, 
        ValueProp props,
        Creature? dealer, 
        CardModel? cardSource)
    {
        if (target != dealer && props.IsPoweredAttack() && result.UnblockedDamage > 0 && target == Owner.Creature)
        {
            await PowerCmd.Apply<DodgePower>(choiceContext, base.Owner.Creature, DynamicVars["DodgePower"].BaseValue, base.Owner.Creature, null);
        }
    }
}