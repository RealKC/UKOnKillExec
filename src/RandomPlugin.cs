using BepInEx;

namespace UKOnKillExec;

[BepInPlugin("kc.UKOnKillExec", "UKOnKillExec", "1.0.0")]
public class UKOnKillExecPlugin : BaseUnityPlugin
{
    private void Awake()
    {
        Logger.LogInfo("UKOnKillExec started...");
    }
}
