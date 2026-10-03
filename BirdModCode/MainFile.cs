using BaseLib.Config;
using BaseLib.Utils;
using BirdMod.BirdModCode.Data;
using BirdMod.BirdModCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace BirdMod.BirdModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "BirdMod"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";
    
    

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
        new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);
    
    public static string GetVersion()
    {
        var mod = ModManager.GetLoadedMods().FirstOrDefault(m => m.manifest?.id == "BirdMod");

        return mod?.manifest?.version ?? "unknown";
    }

    public static void Initialize()
    {
        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
        ModConfigRegistry.Register(ModId, new BirdModConfig());
        Harmony harmony = new(ModId);
        CustomLocTableManager.Register("owie");
        CustomLocTableManager.Register("battle");

        ModManager.OnMetricsUpload += BirdModMetrics.OnMetricsUpload;

        harmony.PatchAll();
    }
}