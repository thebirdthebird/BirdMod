using System.Data;
using BaseLib.Abstracts;
using BirdMod.BirdModCode.Cards.NewCards;
using BirdMod.BirdModCode.DynamicVars;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.SansIMeanSingletons;

public class TalonEffectHolder() : CustomSingletonModel(HookType.Combat)
{
    public static Dictionary<Talon, IEnumerable<DynamicVar>> TalonEffects = new();
    
    public override async Task AfterCardEnteredCombat(CardModel card)
    {
        if (card is Talon t)
        {
            if (!TalonEffects.ContainsKey(t))
            {
                TalonEffects.Add(t, [new DamageVar("0", 4m, ValueProp.Move)]);
            }
            t._dynamicVars?._vars.Clear();
            foreach (var v in TalonEffects[t])
            {
                t._dynamicVars?._vars.Add(v.Name, v);
            }

            t.GenerateLocalization();
        }
    }

    public static async Task ResetTalon(Talon t)
    {
        // MainFile.Logger.Info("Resetting Talon!!!");
        
        if (!TalonEffects.ContainsKey(t))
        {
            TalonEffects.Add(t, [new DamageVar("-99", 4m, ValueProp.Move)]);
        }
        else
        {
            TalonEffects[t] = [new DamageVar("-99", 4m, ValueProp.Move)];
        }
        t._dynamicVars?._vars.Clear();
        foreach (var v in TalonEffects[t])
        {
            t._dynamicVars?._vars.Add(v.Name, v);
        }

        t._energyCost?._base = 0;
        
        // MainFile.Logger.Info("Talon is " + t + " and effects are " + t._dynamicVars?.ToList().ToString());
        t.GenerateLocalization();
    }

    public static async Task AddEffectToTalon(CardModel c, PlayerChoiceContext choiceContext, DynamicVar var)
    {
        var selectTalonFromHand = await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(c.SelectionScreenPrompt, 0, 1),
            context: choiceContext, player: c.Owner, filter: card => card is Talon, source: c);
        if (selectTalonFromHand.First() is not Talon talon) return;
        DynamicVar? dyna = null;
        if (var.Name == "Damage")
        {
            dyna = new DamageVar(TalonEffects[talon].Count()+1.ToString(), var.BaseValue, ValueProp.Move);
        }
        else if (var.Name == "Block")
        {
            dyna = new BlockVar(TalonEffects[talon].Count()+1.ToString(), var.BaseValue, ValueProp.Move);
        }
        else if (var.Name == "Power")
        {
            var fakePowerVar = var as NoTypePowerVar;
            dyna = new NoTypePowerVar(fakePowerVar?.PowerModelVar ?? throw new NoNullAllowedException("Power Model with no brim"), TalonEffects[talon].Count()+1.ToString(), var.BaseValue);
        }
        TalonEffects[talon] =
        [
            .. TalonEffects[talon],
            dyna ?? throw new NoNullAllowedException("DynamicVar is null: I command thee to explode")
        ];
        await ForceTalonUpdate(talon);
        talon.GenerateLocalization();
    }

    public static async Task PullTalonsToHand(PlayerCombatState pcs)
    {
        var cards = pcs.AllCards.OfType<Talon>();
        await CardPileCmd.Add(cards, PileType.Hand);
    }

    public static async Task ForceTalonUpdate(Talon t)
    {
        if (!TalonEffects.ContainsKey(t))
        {
            TalonEffects.Add(t, [new DamageVar(4m, ValueProp.Move)]);
        }
        t._dynamicVars?._vars.Clear();
        foreach (var v in TalonEffects[t])
        {
            t._dynamicVars?._vars.Add(v.Name, v);
        }

        t.GenerateLocalization();
    }
}