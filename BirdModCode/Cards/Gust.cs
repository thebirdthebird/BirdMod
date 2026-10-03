using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Cards;

public class Gust() : BirdModCard(1, CardType.Skill,
    CardRarity.Rare, TargetType.AnyEnemy)
{
    public const string _increaseKey = "Increase";

    public const int _baseDamage = 20;

    public int _currentDamage = _baseDamage;

    public int _increasedDamage;

    [SavedProperty]
    public int CurrentDamage
    {
        get
        {
            return _currentDamage;
        }
        set
        {
            AssertMutable();
            _currentDamage = value;
            base.DynamicVars["StaggeringPower"].BaseValue = _currentDamage;
            // base.DynamicVars.Damage.BaseValue = _currentDamage;
        }
    }

    [SavedProperty]
    public int IncreasedDamage
    {
        get
        {
            return _increasedDamage;
        }
        set
        {
            AssertMutable();
            _increasedDamage = value;
        }
    }

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("Increase", 5m),
        new PowerVar<StaggeringPower>(CurrentDamage),
        new IntVar("StaggeringHits", 6m)
    ];
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            HoverTipFactory.FromPower<StaggeringPower>()
        ] :
        [
            new HoverTip(new LocString("cards", "BIRDMOD-GUST.flavor")),
            HoverTipFactory.FromPower<StaggeringPower>()
        ];
    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
{
    /*ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
    await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
        .WithHitFx("vfx/vfx_attack_slash")
        .Execute(choiceContext);
        */
    if (play != null)
        if (play.Target == null)
            return;
    var staggering = await PowerCmd.Apply<StaggeringPower>(choiceContext, play?.Target == null ? oc.CombatState.GetOpponentsOf(oc) : [play.Target], this.DynamicVars["StaggeringPower"].BaseValue, oc, this, false);
    foreach (var pain in staggering)
    {
        pain.SetHits(this.DynamicVars["StaggeringHits"].IntValue);
    }
    int intValue = base.DynamicVars["Increase"].IntValue;
    BuffFromPlay(intValue);
    (base.DeckVersion as Gust)?.BuffFromPlay(intValue);
}

protected override void OnUpgrade()
{
    base.DynamicVars["Increase"].UpgradeValueBy(3m);
}

protected override void AfterDowngraded()
{
    UpdateDamage();
}

public void BuffFromPlay(int extraDamage)
{
    IncreasedDamage += extraDamage;
    UpdateDamage();
}

public void UpdateDamage()
{
    CurrentDamage = _baseDamage + IncreasedDamage;
}
}