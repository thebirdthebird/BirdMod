using System.Data;
using BaseLib.Abstracts;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Character;
using BirdMod.BirdModCode.Powers;
using Godot;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.Settings;

namespace BirdMod.BirdModCode.Monsters;

public class Spirit() : CustomPetModel(visibleHp: false)
{
    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 166, 150);

    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 166, 150);

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Fur;

    public override string? CustomVisualPath => "res://BirdMod/images/packed/bird_ingame.tscn";
    
    public List<BirdModCard> PseudoDeck = new List<BirdModCard>();
    public List<BirdModCard> PseudoDiscard = new List<BirdModCard>();
    public List<BirdModCard> PseudoHand = new List<BirdModCard>();
    public List<BirdModCard> CardsToPlay = new List<BirdModCard>();
    public int EnergyToGain = 2;
    public int CurrentEnergy = 2;
    public int CardsToDraw = 4;
    public bool Panic = false;
    
    public List<NGridCardHolder?> NCardHolders = new List<NGridCardHolder?>();
    public static float CardIntentY = -115f + 50f;
    public static float cX = (60 + (28));

    public void GenerateCardIntentVisuals()
    {
	    if (NCombatRoom.Instance != null)
	    {
		    NCreature? creatureNode = NCombatRoom.Instance.GetCreatureNode(Creature);
		    Marker2D? specialNode = creatureNode?.GetSpecialNode<Marker2D>("%IntentPos");
		    if (specialNode != null)
		    {
			    int i = 0;
			    for (i = 0; i < CardsToPlay.Count; i++)
			    {
				    var card = CardsToPlay[i];
				    NCard? nCard = NCard.Create(card);
				    // nCard!.Scale = new Vector2(0.25f, 0.25f);
				    NGridCardHolder? nCardHolder = NGridCardHolder.Create(nCard!);
				    nCard!.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
				    NCardHolders.Add(nCardHolder);
				    specialNode.AddChildSafely(nCardHolder);
				    // nCardHolder.Position = positionOffsetSetToUse[i];
				    // ReSharper disable once PossibleLossOfFraction
				    nCardHolder!.Position =
					    new Vector2(
						    (float)((-cX) * (CardsToPlay.Count + PseudoHand.Count - 1) / 2) +
						    ((i - 1) * cX) + cX, CardIntentY);
				    nCardHolder.ReassignToCard(card, PileType.None, null, ModelVisibility.Visible);
				    nCardHolder.Show();
			    }
			    i = CardsToPlay.Count;
			    for (int j = 0; j < PseudoHand.Count; j++)
			    {
				    var card = PseudoHand[j];
				    NCard? nCard = NCard.Create(card);
				    // nCard!.Scale = new Vector2(0.25f, 0.25f);
				    NGridCardHolder? nCardHolder = NGridCardHolder.Create(nCard!);
				    nCard!.UpdateVisuals(PileType.Hand, CardPreviewMode.Normal);
				    NCardHolders.Add(nCardHolder);
				    specialNode.AddChildSafely(nCardHolder);
				    // nCardHolder.Position = positionOffsetSetToUse[i];
				    // ReSharper disable once PossibleLossOfFraction
				    nCardHolder!.Position =
					    new Vector2(
						    (float)((-cX) * (CardsToPlay.Count + PseudoHand.Count - 1) / 2) +
						    ((i - 1) * cX) + cX, CardIntentY);
				    nCardHolder.ReassignToCard(card, PileType.Hand, null, ModelVisibility.Visible);
				    nCardHolder.Modulate = new Color(0.4f, 0.4f, 0.4f, 0.7f);
				    nCardHolder.Show();
				    i++;
			    }
		    }
	    }
    }

    /// <summary>
    /// Look at the Player's Deck, and copy it.
    /// im going insane
    /// </summary>
    /// <param name="theDeckInQuestion"> The Deck in question the Spirit is looking at.</param>
    /// <param name="rng"> The RNG to use.</param>
    public void LookAtMyDeckPlease(List<BirdModCard> theDeckInQuestion, Rng rng)
    {
	    PseudoDeck = theDeckInQuestion.StableShuffle(rng);
    }
    
    public AbstractIntent[] DetermineIntentsFromCards(List<BirdModCard> cards)
    {
	    var intents = new List<AbstractIntent>();
	    foreach (var c in cards)
		{
			var flag = false;
			var addedIntent = false;
			if (c.DynamicVars.Values.OfType<BlockVar>().Any())
			{
				intents.Add(new DefendIntent());
				if (c.DynamicVars.Values.OfType<RepeatVar>().Any())
				{
					flag = true;
					/*
					intents.Add(new DefendIntent());
					
					for (int i = 0; i < c.DynamicVars.Repeat.IntValue; i++)
					{
						intents.Add(new DefendIntent());
					}
					*/
				}
				addedIntent = true;
			}
			if (c.DynamicVars.Values.OfType<DamageVar>().Any() && !addedIntent)
			{
				if (c.DynamicVars.Values.OfType<RepeatVar>().Any() && !flag)
				{
					if (c is ConfidentStrike && this.Creature.HasPower<DodgePower>())
					{
						intents.Add(new MultiAttackIntent(c.DynamicVars.Damage.IntValue,
							1 + c.DynamicVars.Repeat.IntValue));
					}
					else
					{
						intents.Add(new MultiAttackIntent(c.DynamicVars.Damage.IntValue, c.DynamicVars.Repeat.IntValue));
					}
				}
				else
				{
					intents.Add(new SingleAttackIntent(c.DynamicVars.Damage.IntValue));
				}
				addedIntent = true;
			}
			if (c.DynamicVars.Values.OfType<PowerVar<StaggeringPower>>().Any() && !addedIntent)
			{
				if (c is GiantToppler or LetLoose)
				{
					intents.Add(new UnknownIntent());
				}
				else
				{
					if (c.DynamicVars["StaggeringPower"].IntValue > 20)
					{
						intents.Add(new DebuffIntent(true));
					}
					else intents.Add(new DebuffIntent());
				}
				addedIntent = true;
			}
			if (c.Type == CardType.Power && !addedIntent)
			{
				intents.Add(new BuffIntent());
				addedIntent = true;
			}
			if (c is { Type: CardType.Skill, DynamicVars.Count: > 0 } && !addedIntent)
			{
				intents.Add(new BuffIntent());
				addedIntent = true;
			}
			if (!addedIntent && c.Type != CardType.Power && (c.DynamicVars.Count <= 0 || !(c.DynamicVars.Values.OfType<BlockVar>().Any() || c.DynamicVars.Values.OfType<DamageVar>().Any() || c.DynamicVars.Values.OfType<PowerVar<StaggeringPower>>().Any())))
			{
				intents.Add(new UnknownIntent()); // dude?
			}
		}
	    
	    if (intents.Count > 0)
	    {
		    return [.. intents];
	    }
	    //error 
	    return [new UnknownIntent()];
    }
    
    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> list = new List<MonsterState>();
        
        MoveState empty = new MoveState("EMPTY_MOVE", EmptyMove, new AbstractIntent[0]);
        
        empty.FollowUpState = empty;
        list.Add(empty);
        return new MonsterMoveStateMachine(list, empty);
    }

    public void Play(int index, bool discard = true)
    {
	    CardsToPlay.Add(PseudoHand[index]);
	    if (PseudoHand[index].DynamicVars.Values.OfType<EnergyVar>().Any() && PseudoHand[index] is not Retreat)
	    {
		    CurrentEnergy += PseudoHand[index].DynamicVars.Values.OfType<EnergyVar>().FirstOrDefault()!.IntValue;
	    }
	    if (discard == true && !PseudoHand[index].Keywords.Contains(CardKeyword.Exhaust)) PseudoDiscard.Add(PseudoHand[index]);
	    CurrentEnergy -= PseudoHand[index].EnergyCost.Canonical;
	    PseudoHand.RemoveAt(index); 
	    if (PseudoHand.Count == 0) // oh crud you ran out of cards in hand END TURN END TURN
	    {
		    Panic = true;
	    }
    }
    
    public MoveState AreYouReadyToRUMBLEEEE()
    {
	    var bonus = 0;
	    var rng = CombatState.RunState.Rng.CombatCardGeneration;
	    PowerModel? dude = Creature.GetPower<HarnessingEnergyPower>();
	    if (dude != null) bonus = dude.DynamicVars.Values.OfType<EnergyVar>().FirstOrDefault()!.IntValue; 
	    CurrentEnergy = EnergyToGain + Creature.GetPowerAmount<EnergyNextTurnPower>() + bonus; // reset energy to probably 3
	    if (Creature.HasPower<EnergyNextTurnPower>())
	    {
		    PowerCmd.Remove(Creature.GetPower<EnergyNextTurnPower>());
	    }
	    CardsToPlay = new List<BirdModCard>(); // just in case, clear cardstoplay
	    var done = false;
	    var i = 0;
	    var check = 0;
	    
	    // for each card in your hand, look at them with your eyes
	    while (!done)
	    {
		    if (PseudoHand[i].EnergyCost.Canonical > CurrentEnergy)
		    {
			    i += 1;
			    if (i >= PseudoHand.Count - 1)
			    {
				    check += 1;
				    i = 0;
			    }
			    if (check > 8)
			    {
				    //S TOPPP PLEASE E
				    done = true;
			    }
			    if (check > 10 || i > 99) // KILL KILL IT NOWW STOPPP
				    break;
				    
			    continue; // too expensive nerd
		    }
		    
		    if (PseudoHand[i].Type == CardType.Power && rng.NextInt(0, CardsToPlay.Count+1) < 1) // hello my name is the bird and i prioritize playing powers
		    {
			    Play(i, false);
			    i -= 1;
			    if (Panic)
			    {
				    Panic = false;
				    return new MoveState("RANDOM_MOVE", PlayHand, DetermineIntentsFromCards(CardsToPlay));
			    }
		    }
		    else if (check > 1 && PseudoHand[i].Type == CardType.Skill && !PseudoHand[i].DynamicVars.Values.OfType<BlockVar>().Any() && CurrentEnergy > 1)
		    {
			    // this is a skill that doesnt have any block
			    if (PseudoHand[i].DynamicVars.Values.OfType<CardsVar>().Any())
			    {
				    if (PseudoHand[i] is GoPrimal)
				    {
					    PseudoDeck.Add(PseudoCreate<Talon>());
					    PseudoDeck.Add(PseudoCreate<Talon>());
				    }
				    else if (PseudoHand[i] is Tippy)
				    {
					    PseudoHand.Add(PseudoCreate<Tappy>());
				    }
				    else // DRAW
				    {
					    for (int j = 0; j < PseudoHand[i].DynamicVars.Values.OfType<CardsVar>().FirstOrDefault()?.BaseValue; ++j)
					    {
						    if (PseudoDeck.Count == 0 && PseudoDiscard.Count == 0) continue; // HOW
						    if (PseudoDeck.Count == 0) // you need to shuffle
						    {
							    for (int k = 0; k < PseudoDiscard.Count; ++k)
							    {
								    var c = CombatState.RunState.Rng.CombatCardGeneration.NextInt(0,
									    PseudoDiscard.Count);
								    PseudoDeck.Add(PseudoDiscard[c]);
								    PseudoDiscard.RemoveAt(c);
							    }
						    }
						    PseudoHand.Add(PseudoDeck.First());
						    PseudoDeck.RemoveAt(0);
					    }
				    }
			    }
			    Play(i);
			    i -= 1;
			    if (Panic)
			    {
				    Panic = false;
				    return new MoveState("RANDOM_MOVE", PlayHand, DetermineIntentsFromCards(CardsToPlay));
			    }
		    }
		    else if (check > 2 && PseudoHand[i].Type == CardType.Skill)
		    {
			    Play(i);
			    i -= 1;
			    if (Panic)
			    {
				    Panic = false;
				    return new MoveState("RANDOM_MOVE", PlayHand, DetermineIntentsFromCards(CardsToPlay));
			    }
		    }
		    else if (check > 4 && PseudoHand[i].Type == CardType.Attack)
		    {
			    Play(i);
			    i -= 1;
			    if (Panic)
			    {
				    Panic = false;
				    return new MoveState("RANDOM_MOVE", PlayHand, DetermineIntentsFromCards(CardsToPlay));
			    }
		    }
		    else if (check > 6)
		    {
			    Play(i);
			    i -= 1;
			    if (Panic)
			    {
				    Panic = false;
				    return new MoveState("RANDOM_MOVE", PlayHand, DetermineIntentsFromCards(CardsToPlay));
			    }
		    }
		    if (i >= PseudoHand.Count - 1)
		    {
			    check += 1;
			    i = 0;
		    }
		    else i += 1;

		    if (check > 8)
			    done = true;
	    }
	    return new MoveState("RANDOM_MOVE", PlayHand, DetermineIntentsFromCards(CardsToPlay));
    }

    public async Task OnSummon()
    {
	    await PowerCmd.Apply<SpiritSummonPower>(new ThrowingPlayerChoiceContext(), base.Creature, 1m, base.Creature, null);

	    
	    var rng = CombatState.RunState.Rng.CombatCardGeneration;
	    
	    for (int i = 0; i < CardsToDraw + Creature.GetPowerAmount<DrawCardsNextTurnPower>() - Creature.GetPowerAmount<DrawLessNextTurnPower>(); ++i)
	    {
		    if (PseudoDeck.Count == 0 && PseudoDiscard.Count == 0) continue; // HOW
		    if (PseudoDeck.Count == 0) // you need to shuffle
		    {
			    for (int k = 0; k < PseudoDiscard.Count; ++k)
			    {
				    var c = CombatState.RunState.Rng.CombatCardGeneration.NextInt(0,
					    PseudoDiscard.Count);
				    PseudoDeck.Add(PseudoDiscard[c]);
				    PseudoDiscard.RemoveAt(c);
			    }
		    }
		    var num = rng.NextInt(0, PseudoDeck.Count);
		    var card = PseudoDeck[num];
		    PseudoHand.Add(card);
		    PseudoDeck.RemoveAt(num);
	    }
	    
	    var move = AreYouReadyToRUMBLEEEE();
	    move.FollowUpState = new MoveState("EMPTY_MOVE", EmptyMove);
	    SetMoveImmediate(move, true);

	    GenerateCardIntentVisuals();
    }
    
    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
	    if (side == CombatSide.Player) return;
	    var rng = CombatState.RunState.Rng.CombatCardGeneration;
	    
	    for (int i = 0; i < CardsToDraw + Creature.GetPowerAmount<DrawCardsNextTurnPower>() - Creature.GetPowerAmount<DrawLessNextTurnPower>(); ++i)
	    {
		    if (PseudoDeck.Count == 0 && PseudoDiscard.Count == 0) continue; // HOW
		    if (PseudoDeck.Count == 0) // you need to shuffle
		    {
			    for (int k = 0; k < PseudoDiscard.Count; ++k)
			    {
				    var c = CombatState.RunState.Rng.CombatCardGeneration.NextInt(0,
					    PseudoDiscard.Count);
				    PseudoDeck.Add(PseudoDiscard[c]);
				    PseudoDiscard.RemoveAt(c);
			    }
		    }
		    var num = rng.NextInt(0, PseudoDeck.Count);
		    var card = PseudoDeck[num];
		    PseudoHand.Add(card);
		    PseudoDeck.RemoveAt(num);
	    }
	    
	    var move = AreYouReadyToRUMBLEEEE();
	    move.FollowUpState = new MoveState("EMPTY_MOVE", EmptyMove);
	    SetMoveImmediate(move, true);

	    GenerateCardIntentVisuals();
    }

    public override async Task AfterSideTurnEnd(
	    PlayerChoiceContext choiceContext, 
	    CombatSide side, 
	    IEnumerable<Creature> participants)
    {
	    if (side != CombatSide.Player) return;
	    await PlayHand(CombatState.GetOpponentsOf(this.Creature.PetOwner!.Creature));
	    var imstupid = PseudoHand.Count;
	    var j = 0;
	    for (int i = 0; i < imstupid; ++i)
	    {
		    if (!PseudoHand[j].Keywords.Contains(CardKeyword.Retain))
		    {
			    if (!PseudoHand[j].Keywords.Contains(CardKeyword.Ethereal))
			    {
				    PseudoDiscard.Add(PseudoHand[j]);
			    }
			    PseudoHand.RemoveAt(j);
			    j -= 1;
		    }
		    j += 1;
	    }	

	    foreach (var n in NCardHolders)
	    {
		    n?.QueueFree();
	    }
	    NCardHolders.Clear();
    }

    public async Task PlayHand(IReadOnlyList<Creature> targets)
    {
	    if (Creature.CombatState == null) return; // HOW?
	    
	    var list = CardsToPlay.ToList();
	    for (int i = 0; i < list.Count; ++i)
	    {
		    await FalseOnPlayWrapper(new ThrowingPlayerChoiceContext(), CombatState.GetOpponentsOf(this.Creature.PetOwner!.Creature).First(), false, default, list[i], false);
		    CardsToPlay.RemoveAt(0);
		    foreach (var n in NCardHolders)
		    {
			    n?.QueueFree();
		    }
		    NCardHolders.Clear();
		    GenerateCardIntentVisuals();
		    /*
		    if (list[i].Type != CardType.Power && !list[i].Keywords.Contains(CardKeyword.Exhaust))
		    {
			    PseudoDiscard.Add(list[i]);
		    }
		    */
	    }
    }
    
    public async Task EmptyMove(IReadOnlyList<Creature> targets)
    {
    }

    public BirdModCard PseudoCreate<T>() where T : BirdModCard
    {
        BirdModCard? idkWhatImDoing = ModelDb.Card<T>().ToMutable() as BirdModCard;
        if (idkWhatImDoing == null) return new Snowgrave();
        idkWhatImDoing.AfterCreated();
        idkWhatImDoing.Owner = null!;
        idkWhatImDoing.AssertMutable();
        idkWhatImDoing.HELP = true;
        // ICombatState .. doesnt have a way to keep track of cards.,, cant make ,..
        return idkWhatImDoing;
    }
    
    public BirdModCard PseudoCreateAlt(BirdModCard card)
    {
	    BirdModCard? idkWhatImDoing = card.ToMutable() as BirdModCard;
	    if (idkWhatImDoing == null) return new Snowgrave();
	    idkWhatImDoing.AfterCreated();
	    idkWhatImDoing.Owner = null!;
	    idkWhatImDoing.AssertMutable();
	    idkWhatImDoing.HELP = true;
	    // ICombatState .. doesnt have a way to keep track of cards.,, cant make ,..
	    return idkWhatImDoing;
    }
    
    public async Task<int> GeneratePlayCount(ICombatState combatState, Creature? target, CardModel c)
    {
	    //int playCount = (Enchantment?.EnchantPlayCount(BaseReplayCount) ?? BaseReplayCount) + 1;
	    int playCount = 1;
	    playCount = Hook.ModifyCardPlayCount(combatState, c, playCount, target, out List<AbstractModel> modifyingModels);
	    await Hook.AfterModifyingCardPlayCount(combatState, c, modifyingModels);
	    return playCount;
    }

    public async Task FalseOnPlayWrapper(PlayerChoiceContext choiceContext, Creature? target, bool isAutoPlay, ResourceInfo resources, BirdModCard c, bool skipCardPileVisuals = false)
    {
        choiceContext.PushModel(this);
		await CombatManager.Instance.WaitForUnpause();
		Creature? CurrentTarget = target;
		int CurrentPlayIndex = 0;
		if (!isAutoPlay)
		{
			// await CardPileCmd.AddDuringManualCardPlay(this);
		}
		else
		{
			await CardPileCmd.Add(c, PileType.Play, CardPilePosition.Bottom, null, skipCardPileVisuals);
			if (!skipCardPileVisuals)
			{
				await Cmd.CustomScaledWait(0.25f, 0.35f);
			}
		}
		ICombatState combatState = CombatState;
		int playCount = await GeneratePlayCount(combatState, target, c);
		if (Creature.IsDead)
		{
			return;
		}
		ulong playStartTime = Time.GetTicksMsec();
		// CombatId? effectCombatId = CombatManager.Instance.BeginCardOrPotionEffect(Owner);
		try
		{
			await ShowAndDestoryCardAsync(0.5f, c);
			for (int i = 0; i < playCount; i++)
			{
				if (CombatManager.Instance.IsOverOrEnding)
				{
					break;
				}
				CurrentPlayIndex = i;
				if (c.Type == CardType.Power)
				{
					// await PlayPowerCardFlyVfx();
				}
				else if (i > 0)
				{
					NCard? nCard = NCard.FindOnTable(c) ?? NCard.Create(c);
					if (nCard != null)
					{
						await nCard.AnimMultiCardPlay();
					}
				}
				
				BranchingPlayerChoiceContext branchingPlayerChoiceContext = new BranchingPlayerChoiceContext(c, LocalContext.NetId.Value, GameActionType.Combat, choiceContext);
				branchingPlayerChoiceContext.PushModel(c);
				await c.SuperPlay(branchingPlayerChoiceContext, null, Creature);
				if (Creature.IsDead)
				{
					return;
				}
				InvokeExecutionFinished();
			}
		}
		finally
		{
			// await CombatManager.Instance.EndCardOrPotionEffect(effectCombatId, Owner);
			// lol we dont need this anymore! not where WE'RE going...
		}
		if (!skipCardPileVisuals)
		{
			float num = (float)(Time.GetTicksMsec() - playStartTime) / 1000f;
			await Cmd.CustomScaledWait(0.15f - num, 0.3f - num);
		}
		CurrentTarget = null;
		CurrentPlayIndex = 0;
		// c.Played?.Invoke();
		choiceContext.PopModel(this);
    }
    
    public async Task ShowAndDestoryCardAsync(float delayTimeBasedOnIndex, BirdModCard c)
    {
	    Control cardPreviewContainer = NRun.Instance?.GlobalUi.CardPreviewContainer ?? throw new NoNullAllowedException();
	    NCard nCard = NCard.Create(c) ?? throw new NoNullAllowedException();
	    cardPreviewContainer.AddChildSafely(nCard);
	    nCard.UpdateVisuals(PileType.Play, CardPreviewMode.Normal);
	    Tween tween = nCard.CreateTween();
	    
	    tween.TweenProperty(nCard, (NodePath)"position", new Vector2(1920/2, 1080/2), 0.2)
		    //.From(Vector2.Zero)
		    .From(new Vector2(1920-1440, (740-102)))
		    .SetEase(Tween.EaseType.Out)
		    .SetTrans(Tween.TransitionType.Cubic);
	    tween.SetParallel();
	    tween.TweenProperty(nCard, (NodePath)"scale", Vector2.One, 0.2)
		    .From(Vector2.Zero)
		    //.From(new Vector2(600, 0))
		    .SetEase(Tween.EaseType.Out)
		    .SetTrans(Tween.TransitionType.Cubic);
	    tween.SetParallel(false);
	    tween.TweenInterval(delayTimeBasedOnIndex);
	    tween.TweenCallback(Callable.From((Action)(() => { NRun.Instance.GlobalUi.AddChildSafely(NCardExhaustVfx.Create(nCard)!); })));
	    tween.TweenProperty(nCard, (NodePath)"modulate", StsColors.exhaustGray,
		    SaveManager.Instance.PrefsSave.FastMode == FastModeType.Fast ? 0.2 : 0.3);
	    tween.TweenCallback(Callable.From(nCard.QueueFree));

	    await tween.AwaitFinished(nCard);
    }
}