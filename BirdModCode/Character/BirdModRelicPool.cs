using BaseLib.Abstracts;
using BirdMod.BirdModCode.Extensions;
using Godot;

namespace BirdMod.BirdModCode.Character;

public class BirdModRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => BirdMod.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}