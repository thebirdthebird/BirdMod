using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.Powers;
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

public class Ambush() : BirdModCard(1, CardType.Attack,
    CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public Creature CalculationOwner
    {
        get => field ?? Owner?.Creature;
        set => field = value;
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        // .. MakeCalculatedDamage(0, (card, target) => card.Owner.Creature.GetPowerAmount<CounterPower>() + card.Owner.Creature.GetPowerAmount<CounterAllPower>())
        .. MakeCalculatedDamage(0, Bonus)
    ];
    
    private static decimal Bonus(CardModel card, Creature? target)
    {
        if (card is not Ambush ambush)
        {
            //What? How?
            return 0;
        }

        Creature ownerCreature = ambush.CalculationOwner;
        //do logic here
        return ownerCreature.GetPowerAmount<CounterPower>() + ownerCreature.GetPowerAmount<CounterAllPower>();
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<CounterPower>(),
            HoverTipFactory.FromPower<CounterAllPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-AMBUSH.flavor")),
        HoverTipFactory.FromPower<CounterPower>(),
        HoverTipFactory.FromPower<CounterAllPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature ownerCreature)
    {
        CalculationOwner = ownerCreature;
        if (play != null)
            if (play.Target == null)
                return;
        await DamageCmd.Attack(this.DynamicVars.CalculatedDamage)
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: "blunt_attack.mp3")
            .HelpTheCmd(ownerCreature, play, this, Owner)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}