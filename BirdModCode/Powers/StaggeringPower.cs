using BaseLib.Abstracts;
using BaseLib.Audio;
using BaseLib.Extensions;
using BaseLib.Hooks;
using BirdMod.BirdModCode.SansIMeanSingletons;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BirdMod.BirdModCode.Powers;

public class StaggeringPower() : BirdModPower, IHasSecondAmount
{
	private bool _firstSet = true;
	public override PowerType Type => PowerType.Debuff;
	public override PowerStackType StackType => PowerStackType.Counter;
	public override bool AllowNegative => true;
	public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
	public override Color AmountLabelColor => HitsLeft > 1 ? StsColors.red : StsColors.redGlow;
	protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("HitsLeft", 0m)];
	public int HitsOnCreate { get; private set; }
	public int HitsLasted { get; private set; }	

	public int HitsLeft
	{
		get;
		private set
		{
			AssertMutable();
			field = value;
			this.InvokeSecondAmountChanged();
			DynamicVars["HitsLeft"].BaseValue = value;
		}
	}

	public string GetSecondAmount() { return HitsLeft.ToString(); }
	public void SetHits(int hits) 
	{
		AssertMutable();
		HitsLeft = hits;
		if (_firstSet)
		{
			_firstSet = false;
			HitsOnCreate = hits;
		}
	}
	public int GetHits() { AssertMutable(); return HitsLeft; }

	public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext context)
	{
		if (this.HitsLeft < 2 && !_firstSet)
		{
			return Owner.CombatState == null ? base.GetHealthBarForecastSegments(context) : [new HealthBarForecastSegment((int)Hook.ModifyDamage(Owner.CombatState.RunState, base.Owner.CombatState, base.Owner, null, this.Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null, ModifyDamageHookType.All, CardPreviewMode.None, out IEnumerable<AbstractModel> _), new Color(0x00FFFFFF), HealthBarForecastDirection.FromRight)];
		}
		return base.GetHealthBarForecastSegments(context); //default, aka, no health forecast
	}

	public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
		Creature? dealer, CardModel? cardSource)
	{
		if (target == this.Owner && props.IsPoweredAttack() && dealer != null)
		{
			HitsLasted++;
			HitsLeft--;
			if (HitsLeft == 0)
			{
				await PerformBoom(choiceContext);
			}
		}
	}
	public async Task PerformBoom(PlayerChoiceContext choiceContext) //totally not taken from touno
	{
		Flash();
		float toWait = 0f;
		if (ThisOneHandlesTheStaggeringPowerSoundsByTrackingDets.GetDets() > 9)
		{
			float temp = ThisOneHandlesTheStaggeringPowerSoundsByTrackingDets.GetDets() - 9;
			toWait = 0.05f - (temp * 0.005f);
		}
		else { toWait = 0.5f/(float)((ThisOneHandlesTheStaggeringPowerSoundsByTrackingDets.GetDets()*2)+1); }
		if (toWait > 0) await Cmd.Wait(toWait);
		float pitch = 0.6f + ThisOneHandlesTheStaggeringPowerSoundsByTrackingDets.GetDets() * 0.05f;
		if (pitch > 3) pitch = 3;
		ModAudio.PlaySound(new ModSound("res://BirdMod/sounds/snd_rudebuster_hit.wav"), 0f, 1f, 0f, pitch);
		// ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
		await CreatureCmd.Damage(choiceContext, this.Owner, new DamageVar(this.Amount, ValueProp.Unpowered), this.Applier.IsDead? null! : this.Applier);
		var callablePowers = Applier.Powers.OfType<CallablePower>().ToList();
		foreach(var p in callablePowers) { await p.YouGotCalled(choiceContext, HitsLasted, this.Owner); }
		ThisOneHandlesTheStaggeringPowerSoundsByTrackingDets.IncDets();
		if (this.Amount != 0) { await PowerCmd.Remove(this); }
	}
}