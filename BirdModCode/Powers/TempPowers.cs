using BaseLib.Abstracts;
using BirdMod.BirdModCode.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
// ReSharper disable ClassNeverInstantiated.Global

namespace BirdMod.BirdModCode.Powers;


public class InstinctPower() : CustomTemporaryPowerModelWrapper<Instinct, DexterityPower>{ }
public class AdjustPower() : CustomTemporaryPowerModelWrapper<Adjust, DexterityPower> { }
public class PanicPower() : CustomTemporaryPowerModelWrapper<Panic, DexterityPower> { }
public class FeatherDancePower() : CustomTemporaryPowerModelWrapper<FeatherDance, DexterityPower> { }
public class WarnPower() : CustomTemporaryPowerModelWrapper<Warn, DexterityPower> { }
public class BracePower() : CustomTemporaryPowerModelWrapper<Brace, DexterityPower> { }
public class NyoomPower() : CustomTemporaryPowerModelWrapper<Nyoom, DexterityPower> { }
public class StumblePower() : CustomTemporaryPowerModelWrapper<Stumble, DexterityPower> { protected override bool InvertInternalPowerAmount => true; }
public class ChirpDexPower() : CustomTemporaryPowerModelWrapper<Chirp, DexterityPower> { protected override bool InvertInternalPowerAmount => true; }
public class ChirpStrPower() : CustomTemporaryPowerModelWrapper<Chirp, StrengthPower> { protected override bool InvertInternalPowerAmount => true; }
public class ZoomPower() : CustomTemporaryPowerModelWrapper<Zoom, DexterityPower> { }
public class HidePower() : CustomTemporaryPowerModelWrapper<Hide, DexterityPower> { }
public class MomentumDexPower() : CustomTemporaryPowerModelWrapper<MomentumPower, DexterityPower> { }
public class AwakenedFormDexPower() : CustomTemporaryPowerModelWrapper<AwakenedFormPower, DexterityPower> { }
public class TearToShredsPower() : CustomTemporaryPowerModelWrapper<TearToShreds, StrengthPower> { protected override bool InvertInternalPowerAmount => true; }
public class CripplePower() : CustomTemporaryPowerModelWrapper<Cripple, StrengthPower> { protected override bool InvertInternalPowerAmount => true; }
public class FlatterPower() : CustomTemporaryPowerModelWrapper<Flatter, StrengthPower> { protected override bool InvertInternalPowerAmount => true; }
public class TailshakePower() : CustomTemporaryPowerModelWrapper<Tailshake, StrengthPower> { protected override bool InvertInternalPowerAmount => true; }
public class AncestralProtectionStrPower() : CustomTemporaryPowerModelWrapper<AncestralProtectionPower, StrengthPower> { protected override bool InvertInternalPowerAmount => true; }
public class CurbstompStrPower() : CustomTemporaryPowerModelWrapper<CurbstompPower, StrengthPower> { protected override bool InvertInternalPowerAmount => true; }
public class PlatonicKissPower() : CustomTemporaryPowerModelWrapper<PlatonicKiss, DexterityPower> { }
public class StormfrontDexPower() : CustomTemporaryPowerModelWrapper<StormfrontPower, DexterityPower> { }