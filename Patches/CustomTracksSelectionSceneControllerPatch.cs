using HarmonyLib;
using Shared.TrackSelection;
using UnityEngine;
using UnityEngine.UI;

namespace ModMod.Patches;


[HarmonyPatch(typeof(CustomTracksSelectionSceneController))]
public static class RRPlayerPerformanceVFXPatch {
    [HarmonyPatch(typeof(CustomTracksSelectionSceneController), nameof(CustomTracksSelectionSceneController.Awake))]
    [HarmonyPostfix]
    public static void Awake(CustomTracksSelectionSceneController __instance) {
        const float TOP = 800;
        const float STEP = 70;
        
        var canvas = __instance._mainCanvas.transform;
        var screen = (RectTransform)canvas.transform.Find("ScreenContainer");
        
        foreach(var mod in Modifier.GetAll()) {
            var icon = new GameObject(mod.Name, typeof(RectTransform));
            var trans/*rights*/ = icon.GetComponent<RectTransform>();
            trans.SetParent(screen);
            trans.anchorMin = new(0, 0);
            trans.anchorMax = new(0, 0);
            trans.anchoredPosition = new(64, TOP - STEP * mod.Index);
            trans.sizeDelta = new(64, 64);
            
            var img = icon.AddComponent<Image>();
            img.preserveAspect = true;
            
            mod.SetIcon(img);
        }
    }
}
