using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class FeatherDance() : BirdModCard(2,
    CardType.Attack, CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(12, ValueProp.Move),
        new PowerVar<WeakPower>(3m),
        new PowerVar<DexterityPower>(4m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<WeakPower>(),
            HoverTipFactory.FromPower<DexterityPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-FEATHER_DANCE.flavor")),
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        NGrandFinaleVfx nGrandFinaleVfx = NGrandFinaleVfx.Create(oc);
		if (nGrandFinaleVfx != null)
		{
			((Node)(object)NCombatRoom.Instance?.CombatVfxContainer).AddChildSafely((Node?)(object)nGrandFinaleVfx);
			await Cmd.Wait(NGrandFinaleVfx.totalAnticipationDuration);
		}
        if (CombatState == null) return;
        var cmd = DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .HelpTheCmd(oc, play, this, Owner);
        //.WithHitCount(DynamicVars.Repeat.IntValue);
        if (!oc.IsMonster) cmd.TargetingAllOpponents(CombatState);
        await cmd.Execute(choiceContext);
        foreach (Creature hittableEnemy in this.CombatState.GetOpponentsOf(oc))
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, hittableEnemy, this.DynamicVars["WeakPower"].BaseValue, oc, (CardModel)this);
        }
        await PowerCmd.Apply<FeatherDancePower>(choiceContext, oc, base.DynamicVars["DexterityPower"].BaseValue, oc, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(4m);
        base.DynamicVars["WeakPower"].UpgradeValueBy(1m);
        base.DynamicVars["DexterityPower"].UpgradeValueBy(1m);
    }
}