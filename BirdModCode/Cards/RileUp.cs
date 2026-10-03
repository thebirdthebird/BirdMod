using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Cards;

public class RileUp() : BirdModCard(1, CardType.Skill,
    CardRarity.Uncommon, TargetType.AllAllies)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new PowerVar<VigorPower>(7m),
        new PowerVar<VimPower>(-3m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<VimPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-RILE_UP.flavor")),
            HoverTipFactory.FromPower<VigorPower>(),
            HoverTipFactory.FromPower<VimPower>()
        ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (CombatState == null) return;
        IEnumerable<Creature> enumerable = from c in base.CombatState.GetTeammatesOf(base.Owner.Creature)
            where c != null && c.IsAlive && c.IsPlayer
            select c;
        foreach (Creature item in enumerable)
        {
            await PowerCmd.Apply<VigorPower>(choiceContext, item, base.DynamicVars["VigorPower"].BaseValue,
                Owner.Creature, this);
            await PowerCmd.Apply<VimPower>(choiceContext, item, base.DynamicVars["VimPower"].BaseValue,
                Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["VigorPower"].UpgradeValueBy(4m);
    }
}