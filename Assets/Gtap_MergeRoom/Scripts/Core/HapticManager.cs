using System.Collections.Generic;
using Lofelt.NiceVibrations;

public class HapticManager : Singleton<HapticManager>
{
    public bool IsVibration
    {
        get => HapticController.hapticsEnabled;
        set => HapticController.hapticsEnabled = value;
    }
    
    public void Setup(Dictionary<string, bool> settings)
    {
        IsVibration = settings["OnVibration"];
    }

    public void PlayLightHaptic()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
    }
}
