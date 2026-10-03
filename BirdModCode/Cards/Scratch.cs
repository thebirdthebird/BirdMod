using BaseLib.Abstracts;
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
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

  
public class Scratch() : BirdModCard(2,
    CardType.Attack, CardRarity.Basic,
    TargetType.AnyEnemy)
{
    public CardModel GetTranscendenceTransformedCard() => ModelDb.Card<TearToShreds>();
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move),
        new RepeatVar(3),
        new PowerVar<CounterPower>(4m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<CounterPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-SCRATCH.flavor")),
        HoverTipFactory.FromPower<CounterPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_scratch")
            .HelpTheCmd(oc, play, this, Owner)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .Execute(choiceContext);
        await PowerCmd.Apply<CounterPower>(choiceContext, oc, base.DynamicVars["CounterPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(1m);
        base.DynamicVars["CounterPower"].UpgradeValueBy(2m);
    }
}