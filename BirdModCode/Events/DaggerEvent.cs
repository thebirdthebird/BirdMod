using BaseLib.Abstracts;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Events;

public class DaggerEvent : CustomEventModel
{
    
    public override bool IsAllowed(IRunState runState)
    {
        return runState.Players.All((Player p) => p.Character is Character.BirdMod);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
    [
        Option(Plain).ThatDoesDamage(DynamicVars.HpLoss.IntValue),
        Option(Ornate, HoverTipFactory.FromCardWithCardHoverTips<DormantDagger>())
    ];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(12),
        new GoldVar(0),
        new StringVar("Quest", ModelDb.Card<DormantDagger>().Title)
    ];

    public override void CalculateVars()
    {
        DynamicVars.Gold.BaseValue = Rng.NextInt(74, 123);
    }
    
    public async Task Plain()
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner!.Creature, DynamicVars.HpLoss.IntValue, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        await PlayerCmd.GainGold(DynamicVars.Gold.IntValue, Owner);
        SetEventFinished(PageDescription("PLAIN"));
    }

    public async Task Ornate()
    {
        CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(base.Owner!.RunState.CreateCard<DormantDagger>(base.Owner), PileType.Deck));
        SetEventFinished(PageDescription("ORNATE"));
    }

    public override string CustomInitialPortraitPath => ImageHelper.GetImagePath($"events/{ModelDb.Event<ThisOrThat>().Id.Entry.ToLowerInvariant()}.png");
    public override string CustomBackgroundScenePath => SceneHelper.GetScenePath("events/background_scenes/" + ModelDb.Event<ThisOrThat>().Id.Entry.ToLowerInvariant());

}