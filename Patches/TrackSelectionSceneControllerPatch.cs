using HarmonyLib;
using Shared.TrackSelection;

namespace Modifiers.Patches;


[HarmonyPatch(typeof(TrackSelectionSceneController))]
public static class TrackSelectionSceneControllerPatch {
    [HarmonyPatch(typeof(TrackSelectionSceneController), nameof(TrackSelectionSceneController.Update))]
    [HarmonyPostfix]
    public static void TrackSelectUpdate() {
        ModifierKeybinds();
    }
}
