using BaseLib.Abstracts;
using BaseLib.Utils;
using BirdMod.BirdModCode.Character;

namespace BirdMod.BirdModCode.Potions;

[Pool(typeof(BirdModPotionPool))]
public abstract class BirdModPotion : CustomPotionModel;