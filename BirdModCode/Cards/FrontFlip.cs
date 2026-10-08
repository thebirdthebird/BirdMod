using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class FrontFlip() : BirdModCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(8, ValueProp.Move),
        new BlockVar(3, ValueProp.Move),
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-FRONT_FLIP.flavor")),
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await CreatureCmd.GainBlock(oc, this.DynamicVars.Block, play);
        if (play.Target == null) return;
        /*
        List<StaggeringPower> list = [.. play.Target.Powers.OfType<StaggeringPower>()];
        foreach (StaggeringPower sp in list)
        {
            if (sp.GetHits() == 1)
            { 
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, play);
            }
        }
        */
        var num = play.Target.Powers.Count(ShouldCountPower);
        for (int i = 0; i < num; ++i)
        {
            await CreatureCmd.GainBlock(oc, this.DynamicVars.Block, play);
        }
        if (play != null)
            if (play.Target == null)
                return;
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
                .WithHitFx("vfx/vfx_attack_slash")
                .HelpTheCmd(oc, play, this, Owner)
                .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
        base.DynamicVars.Block.UpgradeValueBy(2m);
    }
    
    public static bool ShouldCountPower(PowerModel power)
    {
        if (power.TypeForCurrentAmount == PowerType.Debuff)
        {
            return !(power is ITemporaryPower);
        }
        return false;
    }
}