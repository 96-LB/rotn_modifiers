using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using RhythmRift;
using Shared;
using Shared.Analytics;
using Shared.Pins;
using Shared.SceneLoading;
using Shared.SceneLoading.Payloads;
using Shared.TrackData;
using Shared.TrackSelection;
using Shared.UGC.Local;
using Shared.UGC.Steam;
using Shared.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Modifiers;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ModifiersPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public const string ALLOWED_VERSIONS = "1.12.0 1.11.1 1.10.0 1.8.0 1.7.1 1.7.0";
    public static string[] AllowedVersions => ALLOWED_VERSIONS.Split(' ');

    public static float timer = 0;

    public static Dictionary<string, bool> pin_overrides;
    public static int GloomRowOverride = 3;

    private void Awake()
    {

        pin_overrides = new Dictionary<string, bool>();
        pin_overrides["Gloom"] = false;
        pin_overrides["Enigma"] = false;
        pin_overrides["GlassGuitar"] = false;
        pin_overrides["Perfectionist"] = false;

        
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var gameVersion = BuildInfoHelper.Instance.BuildId.Split('-')[0];

        Logger.LogInfo("Initialising config");

        Modifiers.Config.Initialize(Config);
        Logger.LogInfo("Initialised config");
        if (!AllowedVersions.Contains(gameVersion) && !Modifiers.Config.General.DisableVersionCheck.Value)
        {
            Logger.LogInfo("Invalid game version, ask for an update or disable version check in config");
            return;
        }
        else
        {
            Harmony.CreateAndPatchAll(typeof(ModifiersPlugin));
        }
        Logger.LogInfo("Finished Init"); 
    }

    [HarmonyPatch(typeof(RRStageController), "Update")]
    [HarmonyPostfix]
    public static void StageUpdate()
    {
        ModifierKeybinds();
    }

    [HarmonyPatch(typeof(TrackSelectionSceneController), "Update")]
    [HarmonyPostfix]
    public static void TrackSelectUpdate()
    {
        ModifierKeybinds();
    }

    [HarmonyPatch(typeof(CustomTracksSelectionSceneController), "Update")]
    [HarmonyPostfix]
    public static void CTrackSelectUpdate()
    {
        ModifierKeybinds();
    }
    
    [HarmonyPatch(typeof(RRStageController), "ApplyActivePinEffects")]
    [HarmonyPostfix]
    public static void CustomGloom( RRStageController __instance )
    {
        if (PinsController.IsPinActive("Gloom"))
        {
            for (int i = 0; i < GloomRowOverride; i++)
            {
                Animator animator = __instance._obscureAnimators[i];
                if ((bool)animator)
                {
                    animator.SetTrigger("GloomOn");
                }
            }
        }
    }

    public static void ModifierKeybinds()
    {
        if( UnityInput.Current.GetKeyDown(KeyCode.F1)) pin_overrides["Gloom"] = !pin_overrides["Gloom"];
        if( UnityInput.Current.GetKeyDown(KeyCode.F2)) pin_overrides["Enigma"] = !pin_overrides["Enigma"];
        if( UnityInput.Current.GetKeyDown(KeyCode.F3)) pin_overrides["GlassGuitar"] = !pin_overrides["GlassGuitar"];
        if( UnityInput.Current.GetKeyDown(KeyCode.F4)) pin_overrides["Perfectionist"] = !pin_overrides["Perfectionist"];

        for( int i = 0; i < 9; i++)
        {
            if( UnityInput.Current.GetKeyDown(KeyCode.Alpha1 + i)) GloomRowOverride = i+1;
        }
    }

    [HarmonyPatch(typeof(PinsController), "IsPinActive")]
    [HarmonyPostfix]
    public static void IsPinActive(ref bool __result, string pinName)
    {
        if( pin_overrides.Keys.Contains(pinName))
        {
            __result |= pin_overrides[pinName];
            return;
        }
    }



}
