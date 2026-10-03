using BirdMod.BirdModCode.Cards;
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

namespace BirdMod.BirdModCode.Cards;

public class Tailshake() : BirdModCard(1,
    CardType.Skill, CardRarity.Rare,
    TargetType.AllEnemies)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VimPower>(-6M),
        new PowerVar<DarkShacklesPower>(5M)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<VimPower>(),
            HoverTipFactory.FromPower<StrengthPower>()
        ] :
    [
        new HoverTip(new LocString("cards", "BIRDMOD-TAILSHAKE.flavor")),
        HoverTipFactory.FromPower<VimPower>(),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (oc.CombatState == null) return;
        await PowerCmd.Apply<VimPower>(choiceContext, oc, this.DynamicVars["VimPower"].BaseValue, oc, (CardModel)this);

        foreach (Creature hittableEnemy in oc.CombatState.GetOpponentsOf(oc))
        { 
            await PowerCmd.Apply<TailshakePower>(choiceContext, hittableEnemy, this.DynamicVars["DarkShacklesPower"].BaseValue, oc, (CardModel)this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["VimPower"].UpgradeValueBy(1m);
        base.DynamicVars["DarkShacklesPower"].UpgradeValueBy(2m);
    }
}