using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BirdMod.BirdModCode.Powers;

public class MirrorMovePower() : BirdModPower
{
    public override PowerType Type =>
        PowerType.Buff;

    public override PowerStackType StackType =>
        PowerStackType.Counter;

    private bool flag = false;
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Owner.Player == null || cardPlay.Player == Owner.Player || cardPlay.Card is MirrorMove || flag) return;
        var target = cardPlay.Target is { IsDead: false } t ? t : null;
        if (cardPlay.Card.TargetType == TargetType.Self)
        {
            target = base.Owner;
        }
        if (base.CombatState.GetTeammatesOf(Owner).Any(c => c.Player != Owner.Player && c.HasPower<MirrorMovePower>()))
        {
            flag = true;
        }
        await CardCmd.AutoPlay(choiceContext, cardPlay.Card.CreateDupe(Owner.Player), target);
        await PowerCmd.Decrement(this);
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner))
            return;
        flag = false;
    }
}