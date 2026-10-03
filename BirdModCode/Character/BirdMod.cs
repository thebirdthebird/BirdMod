using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using BirdMod.BirdModCode.Extensions;
using BirdMod.BirdModCode.Cards;
using BirdMod.BirdModCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace BirdMod.BirdModCode.Character;

public class BirdMod : PlaceholderCharacterModel
{
    public const string CharacterId = "BirdMod";

    public static readonly Color Color = new("cdafe9");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 60;

    public override IEnumerable<CardModel> StartingDeck => 
    [
        ModelDb.Card<BirdStrike>(),
        ModelDb.Card<BirdStrike>(),
        ModelDb.Card<BirdStrike>(),
        ModelDb.Card<BirdStrike>(),
        ModelDb.Card<Scratch>(),
        ModelDb.Card<BirdDefend>(),
        ModelDb.Card<BirdDefend>(),
        ModelDb.Card<BirdDefend>(),
        ModelDb.Card<BirdDefend>(),
        ModelDb.Card<Weave>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<TribalFeather>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<BirdModCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BirdModRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BirdModPotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with.
        You don't have to, but you're suggested to rename these images. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    //public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomIconTexturePath => "character_icon_bird.png".CharacterUiPath();
    //public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_bird.png".CharacterUiPath();
    //public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_bird_locked.png".CharacterUiPath();
    //public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_bird.png".CharacterUiPath();

    public override string CustomCharacterSelectBg => "res://BirdMod/images/packed/placeholder_character_select.tscn";
    
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        return NodeFactory<NCreatureVisuals>.CreateFromScene("res://BirdMod/images/packed/bird_ingame.tscn");
    }

    // Below used tool of slay.spencerstiles.com to help
    
    public override Color EnergyLabelOutlineColor => Color;
    public override Color DialogueColor => Color;
    public override Color MapDrawingColor => Color;
    public override Color RemoteTargetingLineColor => Color;
    public override Color RemoteTargetingLineOutline => Color;
    
    private string EnergyCounterLayerPath(int layer)
    {
        return "res://BirdMod/images/packed/energy_counters/birdmod_layer_" + layer + ".png";
    }
    
    public override CustomEnergyCounter? CustomEnergyCounter => new CustomEnergyCounter((Func<int, string>)EnergyCounterLayerPath, new Color("cdafe9"), new Color("cdafe9"));
    
    
    
}