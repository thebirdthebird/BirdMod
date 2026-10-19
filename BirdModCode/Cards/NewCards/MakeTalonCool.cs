using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using BirdMod.BirdModCode.SansIMeanSingletons;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards.NewCards;

public class MakeTalonCool() : BirdModCard(1,
    CardType.Skill, CardRarity.Token,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(4, ValueProp.Move),
        new PowerVar<VimPower>(6),
        new PowerVar<WeakPower>(2)
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        var uhmmdothething = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(base.SelectionScreenPrompt, 1, 1),
            context: choiceContext, player: base.Owner, filter: card => card is Talon, source: this);
        var t = uhmmdothething.First() as Talon;
        if (t is null) return;
        var h = TalonEffectHolder.TalonEffects[t];
        TalonEffectHolder.TalonEffects[t] =
        [
            .. TalonEffectHolder.TalonEffects[t],
            new BlockVar(DynamicVars.Block.IntValue, ValueProp.Move),
            new PowerVar<VimPower>(DynamicVars["VimPower"].IntValue),
            new PowerVar<WeakPower>(DynamicVars["WeakPower"].IntValue)
        ];
        await TalonEffectHolder.ForceTalonUpdate(t);
        t.GenerateLocalization();
    }

    protected override void OnUpgrade()
    {

    }
}