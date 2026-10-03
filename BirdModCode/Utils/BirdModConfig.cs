using BaseLib.Config;

namespace BirdMod.BirdModCode.Utils;

[ConfigHoverTipsByDefault]
public class BirdModConfig : SimpleModConfig
{
    public static bool UploadMetrics { get; set; } = false;

    public static bool ChattyBird { get; set; } = true;
    
    public static bool CloseToDeathMusic { get; set; } = true;

    [ConfigHideInUI]
    [ConfigIgnoreRestoreDefaults]
    public static bool UploadMetricsFtueSeen { get; set; } = false;
}