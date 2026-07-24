using HarmonyLib;
using Shared.TrackSelection;
using UnityEngine;

namespace Modifiers.Patches;


[HarmonyPatch(typeof(CustomTracksSelectionSceneController))]
public static class RRPlayerPerformanceVFXPatch {
    [HarmonyPatch(typeof(CustomTracksSelectionSceneController), nameof(CustomTracksSelectionSceneController.Awake))]
    [HarmonyPostfix]
    public static void Awake(CustomTracksSelectionSceneController __instance) {
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

    [HarmonyPatch(typeof(CustomTracksSelectionSceneController), nameof(CustomTracksSelectionSceneController.Update))]
    [HarmonyPostfix]
    public static void Update() {
        ModifierKeybinds();
    }
}
