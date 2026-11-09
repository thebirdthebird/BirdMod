using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BirdMod.BirdModCode.Powers;

public class ChosenPackTacticPower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Single;
    
    public class Data
    {
        /// <summary>
        /// This will be null for the moment after this power is applied but before this is set by OnPlay.
        /// For all current use cases, this means we should never see this being null.
        /// However, if we needed to override AfterApplied in here in the future, this would be null in it, so let's
        /// leave this nullable for future-proofing.
        /// </summary>
        public CardModel? selectedCard;
    }
    
    protected override object InitInternalData()
    {
        return new Data();
    }
    
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new StringVar("Card")
    ];
    
    public override LocString Title
    {
        get
        {
            var title = new LocString("powers", Id.Entry + ".title");
            foreach (var dynamicVar in DynamicVars.Values)
            {
                title.Add(dynamicVar);
            }

            return title;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips
    {
        get
        {
            List<IHoverTip> list = [];
            var selectedCard = GetInternalData<Data>().selectedCard;

            if (selectedCard == null) return list;
            list.Add(HoverTipFactory.FromCard(selectedCard));
            list.AddRange(selectedCard.HoverTips);

            return list;
        }
    }
    
    public void SetSelectedCard(CardModel card)
    {
        CardModel cardModel = card.CreateClone();
        CardCmd.ClearAffliction(cardModel);
        GetInternalData<Data>().selectedCard = cardModel;
        ((StringVar)base.DynamicVars["Card"]).StringValue = cardModel.Title;
    }
    public const string _cardKey = "Card";
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
        {
            var card = GetInternalData<Data>().selectedCard;
            if (card != null)
            {
                await CardCmd.AutoPlay(choiceContext, card.CreateDupe(player), null!);
            }
        }
    }
}