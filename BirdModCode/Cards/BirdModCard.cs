using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using BirdMod.BirdModCode.Character;
using BirdMod.BirdModCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace BirdMod.BirdModCode.Cards;

/// <summary>
/// This is the base class for your mod's cards, which is set up to load the card's images from your mod's resources.
/// When creating a card, right click the Cards folder and create a new file with the Custom Card template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
[Pool(typeof(BirdModCardPool))]
public abstract class BirdModCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{

    // helper method to grab the tip
    public HoverTip FlavorTip()
    {
        return new HoverTip(new LocString("cards", Id.Entry + ".flavor"));
    }
    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play) //We seal this so we don't accidentally override it later
    {
        await SuperPlay(choiceContext, play, Owner.Creature); //This forces the game to always call your super method instead of the origianl
    }

    public virtual async Task SuperPlay(PlayerChoiceContext pc, CardPlay play, Creature ownerCreature)
    {
        //Override this instead, and use the `ownerCreature` parameter to get the owner instead of `Owner` when writing your cards
    }

    /*
    /// <summary>
    /// Attach additional parameters to any inputted Attack Command.
    /// Meant to bypass FromCard and everything else like that.
    /// I'm going to cry myself to sleep.
    /// </summary>
    /// <param name="ac">Inputted Attack Command.</param>
    /// <param name="oc">The Owner Creature performing the Attack Command.</param>
    /// <param name="play">The CardPlay the Attack Command uses. May be null if called by a Monster.</param>
    /// <returns>Resulting AttackCommand.</returns>
    public AttackCommand HelpTheCmd(AttackCommand ac, Creature oc, CardPlay? play)
    {
        ac.Attacker = oc;
        ac._attackerAnimName = "Attack";
        ac.ModelSource = this;
        if (play != null) ac.CardPlay = play;
        ac._sourceType = AttackCommand.SourceType.Card;
        if (Owner != null) ac._attackerAnimDelay = Owner.Character.AttackAnimDelay;
        if (oc.CombatState == null) return ac;
        if (oc.Monster != null) ac.TargetingAllOpponents(oc.CombatState);
        else
        {
            if (play != null)
                if (play.Target != null)
                    ac.Targeting(play.Target);
        }
        return ac;
    }
    */

// default override so you don't have to include this on cards with no other tips
    protected override IEnumerable<IHoverTip> ExtraHoverTips => IsInCombat
        ?
        [
            
        ]
        :
        [
            FlavorTip()
        ];

    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    // public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string? CustomPortraitPath
    {
        get
        {
            var name = Id.Entry.RemovePrefix().ToLowerInvariant();
            var path = $"res://{MainFile.ModId}/images/card_portraits/big/{name}.png";
            return ResourceLoader.Exists(path) ? path : base.CustomPortraitPath;
        }
    }

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    // public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    // public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}