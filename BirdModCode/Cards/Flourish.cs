using BaseLib.Utils;
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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Flourish() : BirdModCard(1,
    CardType.Attack, CardRarity.Uncommon,
    TargetType.AnyEnemy)
{
    public Creature CalculationOwner
    {
        get => field ?? Owner?.Creature;
        set => field = value;
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        // .. MakeCalculatedDamage(8, (card, target) => card.Owner.Creature.GetPowerAmount<DexterityPower>() + card.Owner.Creature.GetPowerAmount<VimPower>(), 2)
        .. MakeCalculatedDamage(0, Bonus, 2)
    ];
    
    private static decimal Bonus(CardModel card, Creature? target)
    {
        if (card is not Flourish flourish)
        {
            //What? How?
            return 0;
        }

        Creature ownerCreature = flourish.CalculationOwner;
        //do logic here
        return ownerCreature.GetPowerAmount<DexterityPower>() + ownerCreature.GetPowerAmount<VimPower>();
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<DexterityPower>(),
            HoverTipFactory.FromPower<VimPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-FLOURISH.flavor")),
            HoverTipFactory.FromPower<DexterityPower>(),
            HoverTipFactory.FromPower<VimPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        CalculationOwner = oc;
        if (play != null)
            if (play.Target == null)
                return;
        await DamageCmd.Attack(this.DynamicVars.CalculatedDamage)
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: "blunt_attack.mp3")
            .HelpTheCmd(oc, play, this, Owner)
            .Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.CalculationBase.UpgradeValueBy(2m);
        base.DynamicVars.ExtraDamage.UpgradeValueBy(1m);
    }
}