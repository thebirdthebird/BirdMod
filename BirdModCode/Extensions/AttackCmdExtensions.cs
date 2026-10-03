using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace BirdMod.BirdModCode.Extensions;

public static class AttackCmdExtensions //All Extension methods much live inside of a static non-nested class
{
    extension(AttackCommand ac)
    {
        public AttackCommand HelpTheCmd(Creature oc, CardPlay? play, BirdModCard card, Player? ply)
        {
            ac.Attacker = oc;
            ac._attackerAnimName = "Attack";
            ac.ModelSource = card;
            if (play != null) ac.CardPlay = play;
            if (oc.Monster != null) ac._sourceType = AttackCommand.SourceType.Monster;
            else ac._sourceType = AttackCommand.SourceType.Card;
            if (ply != null) ac._attackerAnimDelay = ply.Character.AttackAnimDelay;
            if (oc.CombatState == null) return ac;
            if (oc.Monster != null) ac.TargetingAllOpponents(oc.CombatState);
            else
            {
                if (play != null)
                    if (play.Target != null)
                        if (card.TargetType == TargetType.AllEnemies)
                            ac.TargetingAllOpponents(oc.CombatState);
                        else
                            ac.Targeting(play.Target);
                        
            }
            return ac;
        }
    }
}