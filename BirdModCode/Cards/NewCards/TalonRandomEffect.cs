using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.DynamicVars;
using BirdMod.BirdModCode.Powers;
using BirdMod.BirdModCode.SansIMeanSingletons;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards.NewCards;

public class TalonRandomEffect() : BirdModCard(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        var uhmmdothething = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1, 1),
            context: choiceContext, player: base.Owner, filter: card => card is Talon, source: this);
        var t = uhmmdothething.First() as Talon;
        if (t is null) return;
        if (CombatState == null) return;
        var v = ModelDb.AllPowers.TakeRandom(3, CombatState.RunState.Rng.CombatTargets);
        foreach (var uhm in v)
        {
            TalonEffectHolder.TalonEffects[t] =
            [
                .. TalonEffectHolder.TalonEffects[t],
                new NoTypePowerVar(uhm, CombatState.RunState.Rng.CombatTargets.NextInt(1, 3))
            ];
        }
        
        await TalonEffectHolder.ForceTalonUpdate(t);
        t.GenerateLocalization();
    }

    protected override void OnUpgrade()
    {

    }
}