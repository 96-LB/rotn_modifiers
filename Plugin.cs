using System;
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
using UnityEngine.Networking;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Modifiers;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class ModifiersPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public const string ALLOWED_VERSIONS = "1.15.0 1.14.1 1.12.0 1.11.1 1.10.0 1.8.0 1.7.1 1.7.0";
    public static string[] AllowedVersions => ALLOWED_VERSIONS.Split(' ');

    public static float timer = 0;

    public static Dictionary<string, bool> pin_overrides;
    public static bool customRemix = false;
    public static int GloomRowOverride = 3;

    private static string install_loc = "";

    public static Image gloomImage = null;
    private static Sprite gloomOn = null;
    private static Sprite gloomOff = null;

    public static Image enigmaImage = null;
    private static Sprite enigmaOn = null;
    private static Sprite enigmaOff = null;

    public static Image glassImage = null;
    private static Sprite glassOn = null;
    private static Sprite glassOff = null;

    public static Image perfImage = null;
    private static Sprite perfOn = null;
    private static Sprite perfOff = null;

    public static Image remixImage = null;
    private static Sprite remixOn = null;
    private static Sprite remixOff = null;


    private Sprite load_sprite( string path )
    {
        byte[] gloom_on_bytes = File.ReadAllBytes(install_loc + path);
        Texture2D gloom_tex = new Texture2D(2,2);
        ImageConversion.LoadImage( gloom_tex, gloom_on_bytes  );
        return Sprite.Create( gloom_tex, new Rect(0,0, gloom_tex.width, gloom_tex.height), new Vector2(0.5f,0.5f), 100.0f );
    }

    private void Awake()
    {
        Logger = base.Logger;

        pin_overrides = new Dictionary<string, bool>();
        pin_overrides["Gloom"] = false;
        pin_overrides["Enigma"] = false;
        pin_overrides["GlassGuitar"] = false;
        pin_overrides["Perfectionist"] = false;

        install_loc = Path.GetDirectoryName( base.Info.Location );

        Logger.LogInfo(install_loc + "\\icons\\GloomOn.png");

        gloomOn = load_sprite("\\icons\\GloomOn.png");
        gloomOff = load_sprite("\\icons\\GloomOff.png");

        enigmaOn = load_sprite("\\icons\\EnigmaOn.png");
        enigmaOff = load_sprite("\\icons\\EnigmaOff.png");

        glassOn = load_sprite("\\icons\\GlassGuitarOn.png");
        glassOff = load_sprite("\\icons\\GlassGuitarOff.png");

        perfOn = load_sprite("\\icons\\PerfectsOn.png");
        perfOff = load_sprite("\\icons\\PerfectsOff.png");

        remixOn = load_sprite("\\icons\\RemixOn.png");
        remixOff = load_sprite("\\icons\\RemixOff.png");

        // Plugin startup logic
        
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Logger.LogInfo(install_loc);

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


    [HarmonyPatch(typeof(RRDynamicScenePayload), "FromMetadata")]
    [HarmonyPostfix]
    public static void FromMetadata(RRDynamicScenePayload __result)
    {
        __result.ShouldProcGen = customRemix;
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

    private static Image createIcon(RectTransform screen, float top, float step, float idx, string name)
    {
        GameObject icon = new GameObject(name, typeof(RectTransform));
        icon.transform.SetParent(screen);

        Image img = icon.AddComponent<UnityEngine.UI.Image>();
        img.preserveAspect = true;        
        
        RectTransform trans = (RectTransform)icon.transform;
        trans.anchorMin = new Vector2(0.0f,0.0f);
        trans.anchorMax = new Vector2(.0f,.0f);
        trans.anchoredPosition = new Vector2(64, top - step * idx );
        trans.sizeDelta = new Vector2(64,64);

        return img;
    }

    [HarmonyPatch(typeof(CustomTracksSelectionSceneController), "Awake")]
    [HarmonyPrefix]
    public static void CTrackSelectAwake(CustomTracksSelectionSceneController __instance)
    {
        Transform canvas = __instance._mainCanvas.transform;
        RectTransform screen = (RectTransform)canvas.transform.Find("ScreenContainer");

        float top = 800.0f;
        float step = 70.0f;

        gloomImage = createIcon( screen, top, step, 0, "GloomIcon" ); 
        gloomImage.sprite = pin_overrides["Gloom"] ? gloomOn : gloomOff;
        enigmaImage = createIcon( screen, top, step, 1, "EnigmaIcon" ); 
        enigmaImage.sprite = pin_overrides["Enigma"] ? enigmaOn : enigmaOff;
        glassImage = createIcon( screen, top, step, 2, "GlassIcon" ); 
        glassImage.sprite = pin_overrides["GlassGuitar"] ? glassOn : glassOff;
        perfImage = createIcon( screen, top, step, 3, "PerfIcon" ); 
        perfImage.sprite = pin_overrides["Perfectionist"] ? perfOn : perfOff;
        remixImage = createIcon( screen, top, step, 4, "RemixIcon" ); 
        remixImage.sprite = customRemix ? remixOn : remixOff;
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
        if( UnityInput.Current.GetKeyDown(KeyCode.F1)) {
            pin_overrides["Gloom"] = !pin_overrides["Gloom"];
            if( gloomImage is not null ) gloomImage.sprite = pin_overrides["Gloom"] ? gloomOn : gloomOff;
            
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F2)) {
            pin_overrides["Enigma"] = !pin_overrides["Enigma"];
            if( enigmaImage is not null ) enigmaImage.sprite = pin_overrides["Enigma"] ? enigmaOn : enigmaOff;
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F3)) {
            pin_overrides["GlassGuitar"] = !pin_overrides["GlassGuitar"];
            if( glassImage is not null ) glassImage.sprite = pin_overrides["GlassGuitar"] ? glassOn : glassOff;
        }
        if( UnityInput.Current.GetKeyDown(KeyCode.F4)){
             pin_overrides["Perfectionist"] = !pin_overrides["Perfectionist"];
             if( perfImage is not null ) perfImage.sprite = pin_overrides["Perfectionist"] ? perfOn : perfOff;
        }
        if(UnityInput.Current.GetKeyDown(KeyCode.F5))
        {
            customRemix = !customRemix;
            if( remixImage is not null ) remixImage.sprite = customRemix ? remixOn : remixOff;
        } 

        for( int i = 0; i < 7; i++)
        {
            if( UnityInput.Current.GetKeyDown(KeyCode.Alpha0 + i)) GloomRowOverride = i+1;
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
