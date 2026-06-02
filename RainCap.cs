using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using RainCap.Patches;

namespace RainCap
{
    // first string below is your plugin's GUID, it MUST be unique to any other mod. Read more about it in BepInEx docs. Be sure to update it if you copy this project.
    [BepInPlugin("com.trinagan.raincap", "RainCap", "1.0.0")]
    public class RainCap : BaseUnityPlugin
    {
        public static ManualLogSource LogSource;

        private const string MainSectionName = "Main";
        internal static ConfigEntry<bool> preventRain;
        // BaseUnityPlugin inherits MonoBehaviour, so you can use base unity functions like Awake() and Update()
        private void Awake()
        {
            // save the Logger to public static field so we can use it elsewhere in the project
            LogSource = Logger;
            LogSource.LogInfo("plugin loaded!");
            InitConfiguration();

            // uncomment line(s) below to enable desired example patch, then press F6 to build the project
            // if this solution is properly placed in a YourSPTInstall/Development folder, the compiled plugin will automatically be copied into YourSPTInstall/BepInEx/plugins
            new ChangeGlassesStatePatch().Enable();
            new RainScreenDropsInitPatch().Enable();
            new RainScreenDropsRenderImagePatch().Enable();
        }

        private void InitConfiguration()
        {
            preventRain = Config.Bind(
                MainSectionName,
                "Caps prevent rain drops",
                false,
                "Makes caps prevent screen drops altogether.");
        }
    }
}
