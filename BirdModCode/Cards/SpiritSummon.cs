using System.ComponentModel;
using System.Security.Principal;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Monsters;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Rngs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace BirdMod.BirdModCode.Cards;

public class SpiritSummon() : BirdModCard(1,
    CardType.Power, CardRarity.Event,
    TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        
    ];

    public override async Task SuperPlay(
        PlayerChoiceContext choiceContext,
        CardPlay play, Creature oc)
    {
        // dead pets stay in Allies, so this finds corpses too
        var flower = CombatState!.Allies.FirstOrDefault(c => c.Monster is Spirit && c.PetOwner == Owner);
        if (flower is { IsAlive: true })
        {
            await CreatureCmd.SetMaxAndCurrentHp(flower, 999999999m);
            flower.HpDisplay = HpDisplay.InfiniteWithoutNumbers;
            return;
        }
        
        if (flower is null)
            flower = await Plant(choiceContext);
        else
            Owner.PlayerCombatState!.AddPetInternal(flower);

        await CreatureCmd.SetMaxAndCurrentHp(flower, 999999999m);
        flower.HpDisplay = HpDisplay.InfiniteWithoutNumbers;
    }
    
    async Task<Creature> Plant(PlayerChoiceContext ctx)
    {
        var h = (await PlayerCmd.AddPet<Spirit>(Owner)) ?? throw new InvalidOperationException();
        var flower = h.Monster as Spirit ?? throw new InvalidOperationException();
        var d = Owner.Deck.Cards.ToList();
        List<BirdModCard> help = [.. d.Where(c => c is BirdModCard && c.MultiplayerConstraint != CardMultiplayerConstraint.MultiplayerOnly && c is not SpryStep & c is not Tailwind && c is not GoFeral && c is not Pluck && c is not PackTactics && c is not AwakenedForm && !c.EnergyCost.CostsX).OfType<BirdModCard>()];
        foreach (var c in help)
        {
            c.HELP = true;
        }
        flower.LookAtMyDeckPlease(help, CombatState?.RunState.Rng.GetRng(rngType: RunRngType.CombatCardSelection) ?? throw new InvalidOperationException());
        var room = NCombatRoom.Instance;
        if (room?.GetCreatureNode(flower.Creature) is { } node && room.GetCreatureNode(Owner.Creature) is { } owner)
        {
            node.Position = owner.Position + Vector2.Right * (owner.Hitbox.Size.X * 0.5f + 160f);
            room.SetCreatureIsInteractable(flower.Creature, true);
        }
        await flower.OnSummon();
        return flower.Creature;
    }

    protected override void OnUpgrade()
    {

    }
}