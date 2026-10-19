using BaseLib.Abstracts;
using BaseLib.Patches.Localization;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.DynamicVars;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.Keywords;
using BirdMod.BirdModCode.SansIMeanSingletons;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards.NewCards;

public class Talon() : BirdModCard(0, CardType.Attack,
    CardRarity.Token, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [];
    
    public override IEnumerable<CardKeyword> CanonicalKeywords => [
        
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-TALON.flavor")),
            
        ];

    public void GenerateLocalization()
    {
        List<LocString> locStrings = new();
        
        foreach (var v in DynamicVars) // v is KeyValuePair<string, DynamicVar>
        {
            var h = v.Value; // h is DynamicVar

            if (h is BlockVar)
            {
                var loc = new LocString("talontext", "T_BLOCK");
                loc.AddObj("B", h);
                DynamicVars.AddTo(loc);
                // loc.Add("Block", DynamicVars.Block.IntValue);
                locStrings.Add(loc);
            }
            else if (h is DamageVar)
            {
                LocString dmg = new LocString("null", "null");
                
                if (TargetType == TargetType.AnyEnemy)
                {
                    if (DynamicVars.ContainsKey("Repeat"))
                    {
                        dmg = new LocString("talontext", "T_DMGREPEAT");
                        dmg.AddObj("D", h);
                        DynamicVars.AddTo(dmg);
                        // dmg.Add("Damage", DynamicVars.Damage.IntValue);
                        // dmg.Add("Repeat", DynamicVars.Repeat.IntValue);
                    }
                    else
                    {
                        dmg = new LocString("talontext", "T_DMG");
                        dmg.AddObj("D", h);
                        DynamicVars.AddTo(dmg);
                        // dmg.Add("Damage", DynamicVars.Damage.IntValue);
                    }
                }
                else
                {
                    if (TargetType == TargetType.AllEnemies)
                    {
                        if (DynamicVars.ContainsKey("Repeat"))
                        {
                            dmg = new LocString("talontext", "T_DMGREPEATALL");
                            dmg.AddObj("D", h);
                            DynamicVars.AddTo(dmg);
                            // dmg.Add("Damage", DynamicVars.Damage.IntValue);
                            // dmg.Add("Repeat", DynamicVars.Repeat.IntValue);
                        }
                        else
                        {
                            dmg = new LocString("talontext", "T_DMGALL");
                            dmg.AddObj("D", h);
                            DynamicVars.AddTo(dmg);
                            // dmg.Add("Damage", DynamicVars.Damage.IntValue);
                        }
                    }
                    
                }
        
                locStrings.Add(dmg);
            }
            else if (h.GetType().IsGenericType)
            {
                if (h.GetType().GetGenericTypeDefinition() == typeof(PowerVar<>))
                {
                    var uhm = typeof(ModelDb)
                        .GetMethod(nameof(ModelDb.Power))
                        ?.MakeGenericMethod(h.GetType().GetGenericArguments()[0])
                        .Invoke(null, null) as PowerModel;
                    var help = uhm?.GetTypeForAmount(h.IntValue); // help is PowerType?
                    if (uhm == null) continue;
                    if (help == null) continue;
                    if (help == PowerType.Debuff)
                    {
                        if (TargetType == TargetType.AllEnemies)
                        {
                            // apply power to all enemies
                            var f = new LocString("talontext", "T_APPLYPWRALL");
                            f.AddObj("Power", h);
                            var name = uhm.Title.GetFormattedText();
                            f.Add("PowerName", name);
                            DynamicVars.AddTo(f);
                            locStrings.Add(f);
                        }
                        else
                        {
                            // apply power to single target
                            var f = new LocString("talontext", "T_APPLYPWR");
                            f.AddObj("Power", h);
                            var name = uhm.Title.GetFormattedText();
                            f.Add("PowerName", name);
                            DynamicVars.AddTo(f);
                            locStrings.Add(f);
                        }
                    }
                    else
                    {
                        var f = new LocString("talontext", "T_GAINPWR");
                        f.AddObj("Power", h);
                        var name = uhm.Title.GetFormattedText();
                        f.Add("PowerName", name);
                        DynamicVars.AddTo(f);
                        locStrings.Add(f);
                    }
                }
            }
            else if (h is NoTypePowerVar notype)
            {
                var help = notype.PowerModelVar.GetTypeForAmount(h.IntValue); // help is PowerType?
                if (help == PowerType.Debuff)
                {
                    if (TargetType == TargetType.AllEnemies)
                    {
                        // apply power to all enemies
                        var f = new LocString("talontext", "T_APPLYPWRALL");
                        f.AddObj("Power", h);
                        var name =  notype.PowerModelVar.Title.GetFormattedText();
                        f.Add("PowerName", name);
                        DynamicVars.AddTo(f);
                        locStrings.Add(f);
                    }
                    else
                    {
                        // apply power to single target
                        var f = new LocString("talontext", "T_APPLYPWR");
                        f.AddObj("Power", h);
                        var name = notype.PowerModelVar.Title.GetFormattedText();
                        f.Add("PowerName", name);
                        DynamicVars.AddTo(f);
                        locStrings.Add(f);
                    }
                }
                else
                {
                    var f = new LocString("talontext", "T_GAINPWR");
                    f.AddObj("Power", h);
                    var name = notype.PowerModelVar.Title.GetFormattedText();
                    f.Add("PowerName", name);
                    DynamicVars.AddTo(f);
                    locStrings.Add(f);
                }
            }
        }

        var str = locStrings.Aggregate("After being played, remove all added effects and reset cost to 0.\n", (current, f) => current + (f.GetFormattedText() + "\n"));
        str = str.TrimEnd('\n');

        DescriptionOverrides.CustomizeDescription += delegate(CardModel card, Creature? target, ref string description)
        {
            if (card == this) 
                description = str;
        };
        DescriptionOverrides.CustomizeDescriptionPost += delegate(CardModel card, Creature? target, ref string description)
        {
            if (card == this) 
                description = str;
        };
    }
    

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play != null)
            if (play.Target == null)
                return;

        foreach (var v in DynamicVars) // v is KeyValuePair<string, DynamicVar>
        {
            var h = v.Value; // h is DynamicVar

            if (h is BlockVar)
            {
                await CreatureCmd.GainBlock(oc, base.DynamicVars.Block, play);
            }
            else if (h is DamageVar)
            {
                var cmd = DamageCmd.Attack(h.BaseValue)
                    .WithHitFx("vfx/vfx_scratch", "res://BirdMod/sounds/snd_knight_cut2.wav")
                    .HelpTheCmd(oc, play, this, Owner);
                if (DynamicVars.ContainsKey("Repeat"))
                {
                    cmd.WithHitCount(DynamicVars.Repeat.IntValue);
                }
                await cmd.Execute(choiceContext);
            }
            else if (h.GetType().IsGenericType)
            {
                if (h.GetType().GetGenericTypeDefinition() == typeof(PowerVar<>))
                {
                    var uhm = typeof(ModelDb)
                        .GetMethod(nameof(ModelDb.Power))
                        ?.MakeGenericMethod(h.GetType().GetGenericArguments()[0])
                        .Invoke(null, null) as PowerModel;
                    if (uhm == null) continue;
                    uhm = uhm.ToMutable();
                    var help = uhm?.GetTypeForAmount(h.IntValue); // help is PowerType?
                    if (uhm != null)
                    {
                        if (help != null)
                        {
                            if (help == PowerType.Debuff)
                            {
                                if (TargetType == TargetType.AllEnemies)
                                {
                                    if (CombatState == null) return;
                                    foreach (var o in CombatState.GetOpponentsOf(oc))
                                    {
                                        await PowerCmd.Apply(choiceContext, uhm, o, h.IntValue, oc, this);
                                    }
                                }
                                else
                                {
                                    if (play == null) return;
                                    if (play?.Target == null) return;
                                    await PowerCmd.Apply(choiceContext, uhm, play.Target, h.IntValue, oc, this);
                                }
                            }
                            else
                            {
                                await PowerCmd.Apply(choiceContext, uhm, oc, h.IntValue, oc, this);
                            }
                        }
                    }
                }
            }
            else if (h is NoTypePowerVar notype)
            {
                var mut = notype.PowerModelVar; // short for mutable
                mut = mut.ToMutable();
                var help = mut.GetTypeForAmount(h.IntValue); // help is PowerType?
                if (help == PowerType.Debuff)
                {
                    if (TargetType == TargetType.AllEnemies)
                    {
                        if (CombatState == null) return;
                        foreach (var o in CombatState.GetOpponentsOf(oc))
                        {
                            await PowerCmd.Apply(choiceContext, mut, o, h.IntValue, oc, this);
                        }
                    }
                    else
                    {
                        if (play == null) return;
                        if (play?.Target == null) return;
                        await PowerCmd.Apply(choiceContext, mut, play.Target, h.IntValue, oc, this);
                    }
                }
                else
                {
                    await PowerCmd.Apply(choiceContext, mut, oc, h.IntValue, oc, this);
                }
            }
        }

        await TalonEffectHolder.ResetTalon(this);
    }

    protected override void OnUpgrade()
    {
        // cry
    }
}