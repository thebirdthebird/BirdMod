using BaseLib.Utils;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

[Pool(typeof(QuestCardPool))]
public class DormantDagger() : BirdModCard(1,
    CardType.Attack, CardRarity.Quest,
    TargetType.AnyEnemy)
{
    public const int MaxKills = 6;

    private const string _killsKey = "Kills";

    private int _kills;
    
    public override bool CanBeGeneratedInCombat => false;
    public override int MaxUpgradeLevel => 0;
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(10, ValueProp.Move),
        new DynamicVar("Kills", 6m)
    ];

    public override Texture2D? CustomFrame => ResourceLoader.Load<Texture2D>("res://images/atlases/ui_atlas.sprites/card/card_frame_quest_s.tres");

    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat 
        ?
        [
            HoverTipFactory.Static(StaticHoverTip.Fatal)
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-DORMANT_DAGGER.flavor")),
            new HoverTip(new LocString("cards", "BIRDMOD-DORMANT_DAGGER.credits")),
            HoverTipFactory.Static(StaticHoverTip.Fatal)
        ];
    
    [SavedProperty]
    public int Kills
    {
        get => _kills;
        set
        {
            AssertMutable();
            _kills = value;
            base.DynamicVars["Kills"].BaseValue = MaxKills - Kills;
        }
    }

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        if (play.Target == null) return;
        bool shouldTriggerFatal = play.Target.Powers.All((PowerModel p) => p.ShouldOwnerDeathTriggerFatal());
        if (play != null)
            if (play.Target == null)
                return;
        AttackCommand attackCommand = await DamageCmd.Attack(this.DynamicVars.Damage.BaseValue)
            .WithHitFx("vfx/vfx_attack_slash")
            .HelpTheCmd(oc, play, this, Owner)
            .Execute(choiceContext);
        if (shouldTriggerFatal && attackCommand.Results.SelectMany((List<DamageResult> r) => r).Any((DamageResult r) => r.WasTargetKilled))
        {
            // DO THE STUFF THAT HAPPENS WHEN FATAL TRIGGERS HERE
            if (Kills + 1 < MaxKills + 1)
            {
                //MainFile.Logger.Info("YOU KILLED SOMEONE! YAY!");
                if (base.DeckVersion is DormantDagger dagger) dagger.Kills++;
                Kills++;
            }
        }
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (Kills >= MaxKills)
        {
            if (base.DeckVersion is DormantDagger)
            {
                PlayerCmd.CompleteQuest(base.DeckVersion);
                CardCmd.TransformTo<AwakenedDagger>(base.DeckVersion);
            }
        }
        return base.AfterCombatEnd(room);
    }

    protected override void OnUpgrade()
    {
        
    }
}